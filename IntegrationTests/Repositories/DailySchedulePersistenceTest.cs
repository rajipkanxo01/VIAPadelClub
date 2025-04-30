/*using IntegrationTests.Helpers;
using Microsoft.EntityFrameworkCore;
using Moq;
using UnitTests.Features.Helpers.Factory;
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
using Xunit.Abstractions;
using Assert = Xunit.Assert;

namespace IntegrationTests.Repositories;

public class DailySchedulePersistenceTest
{
    private readonly ITestOutputHelper _testOutputHelper;

    public DailySchedulePersistenceTest(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Fact]
    public async Task SaveAndReload_DailySchedule_PreservesAggregateData()
    {
        // Arrange
        var dailySchedule = DailyScheduleBuilder.CreateValid().BuildAsync().Data;
        await using var context = MyDbContext.SetupContext();

        // Act  
        await MyDbContext.SaveAndClearAsync(dailySchedule, context);
        var schedule = await context.Set<DailySchedule>().FindAsync(dailySchedule.ScheduleId);
        await context.SaveChangesAsync();

        // Assert
        Assert.NotNull(schedule);

        Assert.Equal(dailySchedule.ScheduleId, schedule.ScheduleId);
    }

    /*[Fact]
    public async Task SaveAndReload_ScheduleWithCourtsBookingsVIP_ShouldMatchOriginal()
    {
        // Arrange
        Mock<IDateProvider> dateProviderMock = new();
        Mock<IScheduleFinder> scheduleFinderMock = new();
        Mock<IPlayerFinder> playerFinderMock = new();

        dateProviderMock.Setup(m => m.Today()).Returns(DateOnly.FromDateTime(DateTime.Today));

        await using var context = MyDbContext.SetupContext();

        // create and save court
        var court = Court.Create(CourtName.Create("D1").Data).Data;
        await context.SaveChangesAsync();

        // create and save player
        var player = await PlayerBuilder.CreateValid()
            .WithVIP()
            .BuildAsync();
        await context.SaveChangesAsync();

        // create daily schedule
        ScheduleId scheduleId = ScheduleId.Create();
        var startTime = new TimeOnly(10, 00);
        var endTime = new TimeOnly(18, 00);
        var vipStartTime = new TimeOnly(12, 00);
        var vipEndTime = new TimeOnly(14, 00);


        var dailySchedule = DailyScheduleBuilder.CreateValid()
            .WithScheduleId(scheduleId)
            .WithDate(DateOnly.FromDateTime(DateTime.Today.AddDays(2)))
            .WithTimeRange(startTime, endTime)
            .WithVipTime(vipStartTime, vipEndTime)
            .WithDateProvider(dateProviderMock.Object)
            .WithScheduleFinder(scheduleFinderMock.Object)
            .BuildAsync().Data;
        await context.SaveChangesAsync();


        // load fresh daily schedule from DB
        var reloadedSchedule = await context.Set<DailySchedule>()
            .Include(x => x.listOfBookings)
            .Include(x => x.listOfCourts)
            .FirstAsync(x => x.ScheduleId == scheduleId);

        scheduleFinderMock
            .Setup(m => m.FindSchedule(It.IsAny<ScheduleId>()))
            .Returns(Result<DailySchedule>.Ok(dailySchedule));
        playerFinderMock
            .Setup(m => m.FindPlayer(player.Data.email))
            .Returns(Result<Player>.Ok(player.Data));

        // add court to daily schedule
        var courtFromDb = await context.Set<Court>().FirstAsync(x => x.Name == court.Name);

        reloadedSchedule.AddAvailableCourt(courtFromDb, dateProviderMock.Object, scheduleFinderMock.Object);
        var result = reloadedSchedule.Activate(dateProviderMock.Object);

        await context.SaveChangesAsync();

        // daily schedule after activating and adding court
        var activatedSchedule = await context.Set<DailySchedule>()
            .Include(x => x.listOfBookings)
            .Include(x => x.listOfCourts)
            .FirstAsync(x => x.ScheduleId == scheduleId);

        // Act
        activatedSchedule.BookCourt(player.Data.email, court, vipStartTime, vipEndTime, dateProviderMock.Object,
            playerFinderMock.Object, scheduleFinderMock.Object);

        await context.SaveChangesAsync();

        var finalDailySchedule = await context.Set<DailySchedule>()
            // .Include(schedule => schedule.listOfAvailableCourts)
            .Include(schedule => schedule.listOfBookings)
            .FirstOrDefaultAsync(x => x.ScheduleId == dailySchedule.ScheduleId);

        // Assert
        Assert.NotNull(finalDailySchedule);
        Assert.Equal(1, finalDailySchedule.listOfCourts.Count);
        Assert.Equal(1, finalDailySchedule.listOfBookings.Count);
    }
    #1#

    [Fact]
    public async Task SaveAndReload_ScheduleWithCourtsBookingsVIP_ShouldMatchOriginal()
    {
        // Arrange
        var (context, scheduleRepository, playerRepository) = SetupRepositories();
        var unitOfWork = new UnitOfWork(context);

        var scheduleFinderMock = new Mock<IScheduleFinder>();
        var dateProviderMock = SetupDateProviderMock();

        await using var transaction = await context.Database.BeginTransactionAsync();

        try
        {
            var player = await PersistPlayerAsync(playerRepository, context);
            var playerFinderMock = SetupPlayerFinderMock(player);

            ScheduleId scheduleId = ScheduleId.Create();
            var startTime = new TimeOnly(0, 0);
            var endTime = new TimeOnly(23, 0);
            var vipStartTime = new TimeOnly(12, 0);
            var vipEndTime = new TimeOnly(14, 0);

            var dailySchedule = DailyScheduleBuilder.CreateValid()
                .WithScheduleId(scheduleId)
                .WithDate(DateOnly.FromDateTime(DateTime.Today.AddDays(2)))
                .WithTimeRange(startTime, endTime)
                .WithVipTime(vipStartTime, vipEndTime)
                .WithDateProvider(dateProviderMock.Object)
                .WithScheduleFinder(scheduleFinderMock.Object)
                .BuildAsync();

            await scheduleRepository.AddAsync(dailySchedule.Data);
            await unitOfWork.SaveChangesAsync();
            context.ChangeTracker.Clear();

            scheduleFinderMock.Setup(m => m.FindSchedule(It.IsAny<ScheduleId>()))
                .Returns(Result<DailySchedule>.Ok(dailySchedule.Data));

            var court = Court.Create(CourtName.Create("D1").Data).Data;
            context.Set<Court>().Add(court);
            await unitOfWork.SaveChangesAsync();
            context.ChangeTracker.Clear();

            var loadedSchedule = await context.Set<DailySchedule>()
                .Include(schedule => schedule.listOfBookings)
                .FirstAsync(x => x.ScheduleId == scheduleId);

            var loadedCourt = await context.Set<Court>().FindAsync(court.Name);
            await context.Entry(loadedSchedule!).Collection(ds => ds.listOfCourts).LoadAsync();

            loadedSchedule!.listOfCourts.Add(loadedCourt!);
            loadedSchedule.Activate(dateProviderMock.Object);
            await unitOfWork.SaveChangesAsync();
            context.ChangeTracker.Clear();

            scheduleFinderMock.Setup(m => m.FindSchedule(It.IsAny<ScheduleId>()))
                .Returns(Result<DailySchedule>.Ok(loadedSchedule));

            // Act
            var bookCourt = loadedSchedule.BookCourt(
                player.email, loadedCourt!, vipStartTime, vipEndTime,
                dateProviderMock.Object, playerFinderMock.Object, scheduleFinderMock.Object);

            foreach (var entry in context.ChangeTracker.Entries())
            {
                _testOutputHelper.WriteLine($"{entry.Entity.GetType().Name} - {entry.State}");
            }

            await unitOfWork.SaveChangesAsync();
            context.ChangeTracker.Clear();

            // Assert
            var reloadedSchedule = await context.Set<DailySchedule>()
                .Include(x => x.listOfCourts)
                .Include(x => x.listOfBookings)
                .FirstOrDefaultAsync(x => x.ScheduleId == scheduleId);

            Assert.NotNull(reloadedSchedule);
            Assert.Single(reloadedSchedule.listOfCourts);
            Assert.Single(reloadedSchedule.listOfBookings);
            
            await transaction.CommitAsync();
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    private async Task<Player> PersistPlayerAsync(IPlayerRepository playerRepository, DomainModelContext context)
    {
        var mock = new Mock<IEmailUniqueChecker>();
        mock.Setup(m => m.IsUnique(It.IsAny<string>()))
            .Returns(Task.FromResult(true));

        var player = await PlayerBuilder.CreateValid()
            .WithVIP()
            .WithEmailChecker(mock.Object)
            .BuildAsync();
        await playerRepository.AddAsync(player.Data);
        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();

        return player.Data;
    }

    private static Mock<IDateProvider> SetupDateProviderMock()
    {
        var mock = new Mock<IDateProvider>();
        mock.Setup(m => m.Today()).Returns(DateOnly.FromDateTime(DateTime.Today.AddHours(2)));
        return mock;
    }

    private static Mock<IPlayerFinder> SetupPlayerFinderMock(Player player)
    {
        var mock = new Mock<IPlayerFinder>();
        mock.Setup(m => m.FindPlayer(It.IsAny<Email>()))
            .Returns(Result<Player>.Ok(player));
        return mock;
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
}*/