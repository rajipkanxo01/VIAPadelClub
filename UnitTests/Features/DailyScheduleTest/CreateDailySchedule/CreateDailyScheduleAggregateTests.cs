using VIAPadelClub.Core.Domain.Aggregates.DailySchedules;
using VIAPadelClub.Core.Domain.Aggregates.DailySchedules.Values;
using Xunit;

namespace UnitTests.Features.DailyScheduleTest.CreateDailySchedule;

public class CreateDailyScheduleAggregateTests
{
    [Fact]
    public void Should_Create_DailySchedule_With_Default_Values()
    {
        // Act
        var dailySchedule = DailySchedule.CreateSchedule();

        // Assert
        Assert.NotNull(dailySchedule.scheduleId);
        Assert.Equal(ScheduleStatus.Draft, dailySchedule.status);
        Assert.Empty(dailySchedule.listOfAvailableCourts);
        Assert.Equal(new TimeSpan(15, 0, 0), dailySchedule.availableFrom);
        Assert.Equal(new TimeSpan(22, 0, 0), dailySchedule.availableUntil);
        Assert.Equal(DateTime.Today, dailySchedule.scheduleDate);
    }
}