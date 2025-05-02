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

        // mock email checker
        var emailCheckerMock = new Mock<IEmailUniqueChecker>();
        emailCheckerMock.Setup(m => m.IsUnique(It.IsAny<string>())).Returns(Task.FromResult(true));

        // mock date provider
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

        // mock schedule finder
        var scheduleFinderMock = new Mock<IScheduleFinder>();
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
        var fullName = FullName.Create("Test", "User").Data;
        var profileUrl = ProfileUri.Create("www.profileUri.com/testUser");

        var playerResult = await Player.Register(email.Data, fullName, profileUrl.Data, emailCheckerMock.Object);
        playerResult.Data.ChangeToVipStatus();
        await playerRepository.AddAsync(playerResult.Data);
        await unitOfWork.SaveChangesAsync();


        /*context.ChangeTracker.Clear();

        var scheduleWithCourt = await context.Set<DailySchedule>()
            .AsNoTracking()
            .Include(schedule => schedule.listOfCourts)
            .FirstAsync(schedule => schedule.ScheduleId == scheduleId);

        Assert.NotNull(scheduleWithCourt);
        Assert.Equal(scheduleWithCourt.listOfCourts.Count, 1);*/


        // activate schedule, save it to database


        // create booking, save it to database
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