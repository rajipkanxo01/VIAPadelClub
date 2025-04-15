using IntegrationTests.Helpers;
using Microsoft.EntityFrameworkCore;
using Moq;
using UnitTests.Features.Helpers.Factory;
using VIAPadelClub.Core.Domain.Aggregates.DailySchedules;
using VIAPadelClub.Core.Domain.Aggregates.DailySchedules.Contracts;
using VIAPadelClub.Core.Domain.Aggregates.DailySchedules.Entities;
using VIAPadelClub.Core.Domain.Aggregates.DailySchedules.Values;
using VIAPadelClub.Core.Domain.Aggregates.Players;
using VIAPadelClub.Core.Domain.Aggregates.Players.Contracts;
using VIAPadelClub.Core.Tools.OperationResult;
using Xunit;
using Assert = Xunit.Assert;

namespace IntegrationTests.Repositories;

public class DailySchedulePersistenceTest
{
    [Fact]
    public async Task SaveAndReload_DailySchedule_PreservesAggregateData()
    {
        // Arrange
        var dailySchedule = DailyScheduleBuilder.CreateValid().BuildAsync().Data;
        await using var context = MyDbContext.SetupContext();

        // Act
        await MyDbContext.SaveAndClearAsync(dailySchedule, context);
        var schedule = await context.DailySchedules.FindAsync(dailySchedule.ScheduleId);

        // Assert
        Assert.NotNull(schedule);

        Assert.Equal(dailySchedule.ScheduleId, schedule.ScheduleId);
    }

    [Fact]
    public async Task SaveAndReload_ScheduleWithCourtsBookingsVIP_ShouldMatchOriginal()
    {
        // Arrange
        Mock<IDateProvider> dateProviderMock = new();
        Mock<IScheduleFinder> scheduleFinderMock = new();
        Mock<IPlayerFinder> playerFinderMock = new();

        dateProviderMock.Setup(m => m.Today()).Returns(DateOnly.FromDateTime(DateTime.Today));


        await using var context = MyDbContext.SetupContext();

        var court = Court.Create(CourtName.Create("S1").Data).Data;
        var startTime = new TimeOnly(10, 00);
        var endTime = new TimeOnly(18, 00);
        var vipStartTime = new TimeOnly(12, 00);
        var vipEndTime = new TimeOnly(14, 00);

        ScheduleId scheduleId = ScheduleId.Create();

        var player = await PlayerBuilder.CreateValid()
            .WithVIP()
            .BuildAsync();

        var dailySchedule = DailyScheduleBuilder.CreateValid()
            .WithScheduleId(scheduleId)
            .WithDate(DateOnly.FromDateTime(DateTime.Today.AddDays(2)))
            .WithTimeRange(startTime, endTime)
            .WithVipTime(vipStartTime, vipEndTime)
            .WithDateProvider(dateProviderMock.Object)
            .WithScheduleFinder(scheduleFinderMock.Object)
            .BuildAsync().Data;
        
        scheduleFinderMock
            .Setup(m => m.FindSchedule(It.IsAny<ScheduleId>()))
            .Returns(Result<DailySchedule>.Ok(dailySchedule));
        playerFinderMock
            .Setup(m => m.FindPlayer(player.Data.email))
            .Returns(Result<Player>.Ok(player.Data));
        
        dailySchedule.AddAvailableCourt(court, dateProviderMock.Object, scheduleFinderMock.Object);
        dailySchedule.Activate(dateProviderMock.Object);


        // Act
        dailySchedule.BookCourt(player.Data.email, court, vipStartTime, vipEndTime, dateProviderMock.Object,
            playerFinderMock.Object, scheduleFinderMock.Object);

        await MyDbContext.SaveAndClearAsync(dailySchedule, context);
        var reloaded = await context.DailySchedules
            .Include(schedule => schedule.listOfAvailableCourts)
            .Include(schedule => schedule.listOfBookings)
            .FirstOrDefaultAsync(x => x.ScheduleId == dailySchedule.ScheduleId);

        // Assert
        Assert.NotNull(reloaded);
        Assert.Equal(1, reloaded.listOfBookings.Count);
        // Assert.Equal(1, reloaded.listOfCourts.Count);
    }
}