using IntegrationTests.Helpers;
using Microsoft.EntityFrameworkCore;
using Moq;
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
    // 1. create a new player and save it to database
    /*var email = Email.Create("123456@via.dk");
    var fullName = FullName.Create("Test", "User").Data;
    var profileUrl = ProfileUri.Create("www.profileUri.com/testUser");

    var playerResult = await Player.Register(email.Data, fullName, profileUrl.Data, emailCheckerMock.Object);
    playerResult.Data.ChangeToVipStatus();
    await playerRepository.AddAsync(playerResult.Data);
    await unitOfWork.SaveChangesAsync();*/

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

        var draftSchedule = await scheduleRepository.GetAsync(scheduleId);
        draftSchedule.Data.UpdateScheduleDateAndTime(scheduleDate, scheduleStartTime, scheduleEndTime,
            dateProviderMock.Object);
        await unitOfWork.SaveChangesAsync();

        scheduleFinderMock.Setup(m => m.FindSchedule(It.IsAny<ScheduleId>()))
            .Returns(draftSchedule);

        // 3. create new court, add it to list of court in daily schedule, activate it, save it to database
        var nameResult = CourtName.Create("S1");
        var courtResult = Court.Create(nameResult.Data);

        var updatedSchedule = await scheduleRepository.GetAsync(scheduleId);
        updatedSchedule.Data.AddAvailableCourt(courtResult.Data, dateProviderMock.Object, scheduleFinderMock.Object);
        updatedSchedule.Data.Activate(dateProviderMock.Object);
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

        var activatedSchedule = await GetDailySchedule(context, scheduleId);
        var player = await GetPlayer(context, email.Data);
        var court = activatedSchedule.listOfCourts[0];

        scheduleFinderMock.Setup(m => m.FindSchedule(It.IsAny<ScheduleId>()))
            .Returns(Result<DailySchedule>.Ok(activatedSchedule));
        playerFinderMock.Setup(m => m.FindPlayer(It.IsAny<Email>()))
            .Returns(Result<Player>.Ok(player));

        var bookingResult = activatedSchedule.BookCourt(player.email, court, bookingStarTime, bookingEndTime, dateProviderMock.Object,
            playerFinderMock.Object, scheduleFinderMock.Object);
        await unitOfWork.SaveChangesAsync();
        
        /*context.ChangeTracker.Clear();

        var scheduleWithCourt = await context.Set<DailySchedule>()
            .AsNoTracking()
            .Include(schedule => schedule.listOfCourts)
            .FirstAsync(schedule => schedule.ScheduleId == scheduleId);

             Assert.NotNull(activatedSchedule);
        Assert.Equal(activatedSchedule.Data.listOfCourts.Count(), 1);

        */


        // activate schedule, save it to database


        // create booking, save it to database
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