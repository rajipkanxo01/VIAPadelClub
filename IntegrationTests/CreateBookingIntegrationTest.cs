using IntegrationTests.Helpers;
using Moq;
using VIAPadelClub.Core.Domain.Aggregates.DailySchedules;
using VIAPadelClub.Core.Domain.Aggregates.DailySchedules.Contracts;
using VIAPadelClub.Core.Domain.Aggregates.DailySchedules.Values;
using VIAPadelClub.Core.Domain.Aggregates.Players;
using VIAPadelClub.Core.Domain.Aggregates.Players.Contracts;
using VIAPadelClub.Core.Domain.Aggregates.Players.Values;
using VIAPadelClub.Infrastructure.EfcDmPersistence;
using VIAPadelClub.Infrastructure.EfcDmPersistence.Repositories;
using Xunit;

namespace IntegrationTests;

public class CreateBookingIntegrationTest
{
    [Fact]
    public async Task ShouldCreateAndSaveBookingToDatabase_WhenValid()
    {
        var (context, scheduleRepository, playerRepository) = SetupRepositories();
        var unitOfWork = new UnitOfWork(context);

        // mock email checker
        var emailCheckerMock = new Mock<IEmailUniqueChecker>();
        emailCheckerMock.Setup(m => m.IsUnique(It.IsAny<string>())).Returns(Task.FromResult(true));

        // mock date provider
        var dateProviderMock = new Mock<IDateProvider>();
        dateProviderMock.Setup(m => m.Today()).Returns(DateOnly.FromDateTime(DateTime.Today.AddDays(2).AddHours(2)));

        // 1. create a new player and save it to database
        var email = Email.Create("123456@via.dk");
        var fullName = FullName.Create("Test", "User").Data;
        var profileUrl = ProfileUri.Create("www.profileUri.com/testUser");
        
        var playerResult = await Player.Register(email.Data, fullName, profileUrl.Data, emailCheckerMock.Object);
        playerResult.Data.ChangeToVipStatus();
        await playerRepository.AddAsync(playerResult.Data);
        await unitOfWork.SaveChangesAsync();

        // 2. create daily schedule, save it to database
        var scheduleId = ScheduleId.Create();
        var scheduleResult = DailySchedule.CreateSchedule(dateProviderMock.Object, scheduleId);
        await scheduleRepository.AddAsync(scheduleResult.Data);
        await unitOfWork.SaveChangesAsync();


        // create new court, save it to database


        // add court save it to database


        // activate schedule, save it to database


        // create booking, save it to database
    }

    private static (DomainModelContext context, IDailyScheduleRepository scheduleRepository, IPlayerRepository playerRepository) SetupRepositories()
    {
        var context = MyDbContext.SetupContext();
        return (
            context,
            new DailyScheduleRepository(context),
            new PlayerRepository(context)
        );
    }
}