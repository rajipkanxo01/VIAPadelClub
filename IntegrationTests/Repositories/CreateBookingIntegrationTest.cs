using IntegrationTests.Helpers;
using Microsoft.EntityFrameworkCore;
using Moq;
using Services.Contracts;
using VIAPadelClub.Core.Application.CommandDispatching.Commands.DailySchedule;
using VIAPadelClub.Core.Application.Features.Daily_Schedule;
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

namespace IntegrationTests.Repositories;

public class CreateBookingIntegrationTest
{
    [Fact]
    public async Task ShouldCreateDailyScheduleAndSaveToDatabase_WhenValid()
    {
        var (context, scheduleRepository, playerRepository) = SetupRepositories();
        var unitOfWork = new UnitOfWork(context);

        // mock objects
        var scheduleFinderMock = new Mock<IScheduleFinder>();
        var playerFinderMock = new Mock<IPlayerFinder>();

        var emailCheckerMock = new Mock<IEmailUniqueChecker>();
        emailCheckerMock.Setup(m => m.IsUnique(It.IsAny<string>())).Returns(Task.FromResult(true));

        var dateProviderMock = new Mock<IDateProvider>();
        dateProviderMock.Setup(m => m.Today()).Returns(DateOnly.FromDateTime(DateTime.Today.AddDays(2).AddHours(2)));

        // 1. create daily schedule, save it to database
        var scheduleId = ScheduleId.Create();
        var scheduleResult = DailySchedule.CreateSchedule(dateProviderMock.Object, scheduleId);
        await scheduleRepository.AddAsync(scheduleResult.Data);
        await unitOfWork.SaveChangesAsync();

        // 2. Get daily schedule from database, update date and time, update it in database
        var scheduleStartTime = new TimeOnly(00, 30);
        var scheduleEndTime = new TimeOnly(23, 30);
        var scheduleDate = DateOnly.FromDateTime(DateTime.Today.AddDays(3));

        scheduleResult.Data.UpdateScheduleDateAndTime(scheduleDate, scheduleStartTime, scheduleEndTime,
            dateProviderMock.Object);
        await unitOfWork.SaveChangesAsync();

        // 3. create new court, add it to list of court in daily schedule, activate it, save it to database
        var nameResult = CourtName.Create("S1");
        var courtResult = Court.Create(nameResult.Data);

        scheduleFinderMock.Setup(m => m.FindSchedule(It.IsAny<ScheduleId>()))
            .Returns(scheduleResult);
        scheduleResult.Data.AddAvailableCourt(courtResult.Data, dateProviderMock.Object, scheduleFinderMock.Object);
        scheduleFinderMock.Setup(m => m.FindSchedule(It.IsAny<ScheduleId>()))
            .Returns(scheduleResult);

        scheduleResult.Data.Activate(dateProviderMock.Object);
        await unitOfWork.SaveChangesAsync();

        // 4. create player, save it to database
        var email = Email.Create("123456@via.dk");
        var fullName = FullName.Create("Test", "User");
        var profileUrl = ProfileUri.Create("www.profileUri.com/testUser");

        var playerResult = await Player.Register(email.Data, fullName.Data, profileUrl.Data, emailCheckerMock.Object);
        await playerRepository.AddAsync(playerResult.Data);

        await unitOfWork.SaveChangesAsync();

        // 5. create booking, save it to database
        var bookingStarTime = new TimeOnly(12, 00);
        var bookingEndTime = new TimeOnly(13, 00);
        var court = scheduleResult.Data.listOfCourts[0];

        var player = await GetPlayer(context, email.Data);
        
        scheduleFinderMock.Setup(m => m.FindSchedule(It.IsAny<ScheduleId>()))
            .Returns(scheduleResult);
        playerFinderMock.Setup(m => m.FindPlayer(It.IsAny<Email>()))
            .Returns(Result<Player>.Ok(player));

        scheduleResult.Data.BookCourt(player.email, court, bookingStarTime, bookingEndTime,
            dateProviderMock.Object,
            playerFinderMock.Object, scheduleFinderMock.Object);

        await unitOfWork.SaveChangesAsync();

        var dailySchedule = await GetDailySchedule(context, scheduleId);
        
        // Assert
        Assert.NotNull(dailySchedule);
        Assert.Equal(scheduleId, dailySchedule.ScheduleId);
        Assert.Equal(scheduleDate, dailySchedule.scheduleDate);
        Assert.NotEmpty(dailySchedule.listOfBookings);
        Assert.Equal(1, dailySchedule.listOfBookings.Count);
    }

    private static async Task<DailySchedule> GetDailySchedule(DomainModelContext context, ScheduleId scheduleId)
    {
        var dailySchedule = await context.Set<DailySchedule>()
            .AsNoTracking()
            .Include(schedule => schedule.listOfCourts)
            .Include(schedule => schedule.listOfBookings)
            .FirstAsync(schedule => schedule.ScheduleId == scheduleId);
        return dailySchedule;
    }

    private static async Task<Player> GetPlayer(DomainModelContext context, Email email)
    {
        var player = await context.Set<Player>()
            .AsNoTracking()
            .FirstAsync(player => player.email == email);

        return player;
    }
    
    private static (DomainModelContext context, IDailyScheduleRepository scheduleRepository, IPlayerRepository
        playerRepository) SetupRepositories()
    {
        var context = MyDbContext.SetupContext();
        return (
            context,
            new DailyScheduleRepository(context),
            new PlayerRepository(context)
        );
    }
}
