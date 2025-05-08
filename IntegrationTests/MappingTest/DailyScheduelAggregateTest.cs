using IntegrationTests.Helpers;
using Microsoft.EntityFrameworkCore;
using Moq;
using Services.Contracts;
using UnitTests.Features.Helpers;
using UnitTests.Features.Helpers.Repository;
using VIAPadelClub.Core.Domain.Aggregates.DailySchedules;
using VIAPadelClub.Core.Domain.Aggregates.DailySchedules.Contracts;
using VIAPadelClub.Core.Domain.Aggregates.DailySchedules.Entities;
using VIAPadelClub.Core.Domain.Aggregates.DailySchedules.Values;
using VIAPadelClub.Core.Domain.Aggregates.Players;
using VIAPadelClub.Core.Domain.Aggregates.Players.Contracts;
using VIAPadelClub.Core.Domain.Aggregates.Players.Values;
using VIAPadelClub.Core.Tools.OperationResult;
using VIAPadelClub.Infrastructure.EfcDmPersistence;
using VIAPadelClub.Infrastructure.EfcDmPersistence.Repositories;
using Xunit;

namespace IntegrationTests.MappingTest;

[Collection("Sequential")]
public class DailyScheduleAggregateTest
{

    [Fact]
    public async Task ShouldSucceed_WhenOneToManyRelationship_betweenDailyScheduleAndBooking_IsVerified()
    {
        // Arrange
        var fixture = new TestFixture();

        // Create schedule with date and time
        var scheduleId = ScheduleId.Create();
        var scheduleResult = await fixture.CreateAndSaveSchedule(scheduleId);
        scheduleResult = await fixture.UpdateScheduleDateAndTime(scheduleResult);

        // Add courts to schedule
        var court1 = await fixture.AddCourtToSchedule(scheduleResult, "S1");
        var court2 = await fixture.AddCourtToSchedule(scheduleResult, "S2");

        // Activate schedule
        scheduleResult.Data.Activate(fixture.DateProviderMock.Object);
        await fixture.UnitOfWork.SaveChangesAsync();

        // Create players
        var player1 = await fixture.CreateAndSavePlayer("123456@via.dk");
        var player2 = await fixture.CreateAndSavePlayer("user@via.dk");

        // Create bookings
        var booking1 = await fixture.BookCourt(
            scheduleResult,
            player1.email,
            court1,
            new TimeOnly(10, 00),
            new TimeOnly(11, 00));

        var booking2 = await fixture.BookCourt(
            scheduleResult,
            player2.email,
            court2,
            new TimeOnly(12, 00),
            new TimeOnly(13, 00));

        // Add bookings to schedule
        scheduleResult.Data.listOfBookings.Add(booking1.Data);
        scheduleResult.Data.listOfBookings.Add(booking2.Data);
        await fixture.UnitOfWork.SaveChangesAsync();

        // Act
        var dailySchedule = await GetDailySchedule(fixture.Context, scheduleId);

        // Assert
        Assert.Equal(2, dailySchedule.listOfBookings.Count);
    }

    [Fact]
    public async Task ShouldReferenceCourtCorrectly_WhenBookingIsCreated_AsBookingAndCourtHasOneToOneRelation()
    {
        // Arrange
        var fixture = new TestFixture();

        // Create schedule with date and time
        var scheduleId = ScheduleId.Create();
        var scheduleResult = await fixture.CreateAndSaveSchedule(scheduleId);
        scheduleResult = await fixture.UpdateScheduleDateAndTime(scheduleResult);

        // Add court to schedule
        var court = await fixture.AddCourtToSchedule(scheduleResult, "S1");

        // Activate schedule
        scheduleResult.Data.Activate(fixture.DateProviderMock.Object);
        await fixture.UnitOfWork.SaveChangesAsync();

        // Create player
        var player = await fixture.CreateAndSavePlayer();

        // Create booking
        var booking = await fixture.BookCourt(
            scheduleResult,
            player.email,
            court,
            new TimeOnly(12, 00),
            new TimeOnly(13, 00));

        // Act
        var retrievedBooking = await fixture.Context.Set<Booking>()
            .AsNoTracking()
            .Include(b => b.Court)
            .SingleAsync(b => b.BookingId == booking.Data.BookingId);

        // Assert
        Assert.Equal(court.Name, retrievedBooking.Court.Name);
        Assert.Equal(scheduleId, retrievedBooking.Court.ScheduleId);
    }

    [Fact]
    public async Task ShouldCorrectly_AssociatePlayerWithBooking_WhenBookingIsCreated_AsPlayerAndBookingHasOneToManyRelationship()
    {
        // Arrange
        var fixture = new TestFixture();

        // Create schedule with date and time
        var scheduleId = ScheduleId.Create();
        var scheduleResult = await fixture.CreateAndSaveSchedule(scheduleId);
        scheduleResult = await fixture.UpdateScheduleDateAndTime(scheduleResult);

        // Add court to schedule
        var court = await fixture.AddCourtToSchedule(scheduleResult, "S1");

        // Activate schedule
        scheduleResult.Data.Activate(fixture.DateProviderMock.Object);
        await fixture.UnitOfWork.SaveChangesAsync();

        // Create player
        var email = Email.Create("123456@via.dk");
        var player = await fixture.CreateAndSavePlayer(email.Data.Value);

        // Create booking
        var booking = await fixture.BookCourt(
            scheduleResult,
            player.email,
            court,
            new TimeOnly(12, 00),
            new TimeOnly(13, 00));

        // Act
        var retrievedBooking = await fixture.Context.Set<Booking>()
            .AsNoTracking()
            .Include(b => b.Court)
            .SingleAsync(b => b.BookingId == booking.Data.BookingId);

        // Assert
        Assert.Equal(player.email.Value, retrievedBooking.BookedBy.Value);
    }

    [Fact]
    public async Task ShouldPersist_DailyScheduleEntity_WhenNotNull()
    {
        // Arrange
        await using MyDbContext ctx = MyDbContext.SetupContext();
        var todayDate = DateOnly.FromDateTime(DateTime.Today);
        var fakeDateProvider = new FakeDateProvider(todayDate);
        var id = Guid.NewGuid();
        ScheduleId scheduleId = ScheduleId.FromGuid(id);

        var expectedAvailableFrom = new TimeOnly(15, 0);
        var expectedAvailableUntil = new TimeOnly(22, 0);
        var expectedDate = todayDate;

        // Act
        DailySchedule entity = DailySchedule.CreateSchedule(fakeDateProvider, scheduleId).Data;
        await MyDbContext.SaveAndClearAsync(entity, ctx);

        // Act
        DailySchedule retrieved = ctx.Set<DailySchedule>().Single(x => x.ScheduleId.Equals(scheduleId));

        // Assert
        Assert.Equal(scheduleId.Value, retrieved.ScheduleId.Value);
        Assert.Equal(expectedAvailableFrom, retrieved.availableFrom);
        Assert.Equal(expectedAvailableUntil, retrieved.availableUntil);
        Assert.Equal(expectedDate, retrieved.scheduleDate);
        Assert.False(retrieved.isDeleted);
    }

    [Fact]
    public async Task Booking_Should_PersistAndRetrieve_ForeignKeys_Correctly()
    {
        // Arrange
        var fixture = new TestFixture();

        // Create schedule with date and time
        var scheduleId = ScheduleId.Create();
        var scheduleResult = await fixture.CreateAndSaveSchedule(scheduleId);
        scheduleResult = await fixture.UpdateScheduleDateAndTime(scheduleResult);

        // Add court to schedule
        var courtName = "S1";
        var nameResult = CourtName.Create(courtName);
        var court = await fixture.AddCourtToSchedule(scheduleResult, courtName);

        // Activate schedule
        scheduleResult.Data.Activate(fixture.DateProviderMock.Object);
        await fixture.UnitOfWork.SaveChangesAsync();

        // Create player
        var emailValue = "123456@via.dk";
        var email = Email.Create(emailValue);
        var player = await fixture.CreateAndSavePlayer(emailValue);

        // Create booking
        var booking = await fixture.BookCourt(
            scheduleResult,
            player.email,
            court,
            new TimeOnly(12, 00),
            new TimeOnly(13, 00));

        // Act
        var retrievedBooking = await fixture.Context.Set<Booking>()
            .AsNoTracking()
            .Include(b => b.Court)
            .SingleAsync(b => b.BookingId == booking.Data.BookingId);

        // Assert - Foreign keys: BookedBy(Email), Name(CourtName), ScheduleId(ScheduleId)
        Assert.Equal(email.Data, retrievedBooking.BookedBy);
        Assert.Equal(nameResult.Data, retrievedBooking.Court.Name);
        Assert.Equal(scheduleId, retrievedBooking.Court.ScheduleId);
    }

    private class TestFixture
    {
        public DomainModelContext Context { get; }
        public IDailyScheduleRepository ScheduleRepository { get; }
        public IPlayerRepository PlayerRepository { get; }
        public UnitOfWork UnitOfWork { get; }
        public Mock<IScheduleFinder> ScheduleFinderMock { get; }
        public Mock<IPlayerFinder> PlayerFinderMock { get; }
        public Mock<IEmailUniqueChecker> EmailCheckerMock { get; }
        public Mock<IDateProvider> DateProviderMock { get; }

        public TestFixture()
        {
            Context = MyDbContext.SetupContext();
            ScheduleRepository = new DailyScheduleRepository(Context);
            PlayerRepository = new PlayerRepository(Context);
            UnitOfWork = new UnitOfWork(Context);

            ScheduleFinderMock = new Mock<IScheduleFinder>();
            PlayerFinderMock = new Mock<IPlayerFinder>();

            EmailCheckerMock = new Mock<IEmailUniqueChecker>();
            EmailCheckerMock.Setup(m => m.IsUnique(It.IsAny<string>())).Returns(Task.FromResult(true));

            DateProviderMock = new Mock<IDateProvider>();
            DateProviderMock.Setup(m => m.Today()).Returns(DateOnly.FromDateTime(DateTime.Today.AddDays(2).AddHours(2)));
        }

        // Helper method to create and save a schedule
        public async Task<Result<DailySchedule>> CreateAndSaveSchedule(ScheduleId scheduleId = null)
        {
            scheduleId ??= ScheduleId.Create();
            var scheduleResult = DailySchedule.CreateSchedule(DateProviderMock.Object, scheduleId);
            await ScheduleRepository.AddAsync(scheduleResult.Data);
            await UnitOfWork.SaveChangesAsync();
            return scheduleResult;
        }

        // Helper method to update schedule date and time
        public async Task<Result<DailySchedule>> UpdateScheduleDateAndTime(
            Result<DailySchedule> scheduleResult,
            DateOnly? date = null,
            TimeOnly? startTime = null,
            TimeOnly? endTime = null)
        {
            var scheduleDate = date ?? DateOnly.FromDateTime(DateTime.Today.AddDays(3));
            var scheduleStartTime = startTime ?? new TimeOnly(00, 30);
            var scheduleEndTime = endTime ?? new TimeOnly(23, 30);

            scheduleResult.Data.UpdateScheduleDateAndTime(scheduleDate, scheduleStartTime, scheduleEndTime, DateProviderMock.Object);
            await UnitOfWork.SaveChangesAsync();
            return scheduleResult;
        }

        // Helper method to add a court to a schedule
        public async Task<Court> AddCourtToSchedule(Result<DailySchedule> scheduleResult, string courtName = "S1")
        {
            var nameResult = CourtName.Create(courtName);
            var courtResult = Court.Create(nameResult.Data);

            ScheduleFinderMock.Setup(m => m.FindSchedule(It.IsAny<ScheduleId>()))
                .Returns(scheduleResult);

            scheduleResult.Data.AddAvailableCourt(courtResult.Data, DateProviderMock.Object, ScheduleFinderMock.Object);

            return courtResult.Data;
        }

        // Helper method to create and save a player
        public async Task<Player> CreateAndSavePlayer(string email = "123456@via.dk")
        {
            var emailResult = Email.Create(email);
            var fullName = FullName.Create("Test", "User");
            var profileUrl = ProfileUri.Create("www.profileUri.com/testUser");

            var playerResult = await Player.Register(emailResult.Data, fullName.Data, profileUrl.Data, EmailCheckerMock.Object);
            await PlayerRepository.AddAsync(playerResult.Data);
            await UnitOfWork.SaveChangesAsync();

            return await GetPlayer(Context, emailResult.Data);
        }

        // Helper method to book a court
        public async Task<Result<Booking>> BookCourt(
            Result<DailySchedule> scheduleResult,
            Email playerEmail,
            Court court,
            TimeOnly startTime,
            TimeOnly endTime)
        {
            var player = await GetPlayer(Context, playerEmail);

            ScheduleFinderMock.Setup(m => m.FindSchedule(It.IsAny<ScheduleId>()))
                .Returns(scheduleResult);

            PlayerFinderMock.Setup(m => m.FindPlayer(It.IsAny<Email>()))
                .Returns(Result<Player>.Ok(player));

            var bookingResult = scheduleResult.Data.BookCourt(
                player.email,
                court,
                startTime,
                endTime,
                DateProviderMock.Object,
                PlayerFinderMock.Object,
                ScheduleFinderMock.Object);

            await UnitOfWork.SaveChangesAsync();
            return bookingResult;
        }
    }

    // Helper method to get a player from the database
    private static async Task<Player> GetPlayer(DomainModelContext context, Email email)
    {
        var player = await context.Set<Player>()
            .AsNoTracking()
            .FirstAsync(player => player.email == email);

        return player;
    }

    // Helper method to get a daily schedule from the database including its courts and bookings
    private static async Task<DailySchedule> GetDailySchedule(DomainModelContext context, ScheduleId scheduleId)
    {
        var dailySchedule = await context.Set<DailySchedule>()
            .AsNoTracking()
            .Include(schedule => schedule.listOfCourts)
            .Include(schedule => schedule.listOfBookings)
            .FirstAsync(schedule => schedule.ScheduleId == scheduleId);
        return dailySchedule;
    }


}