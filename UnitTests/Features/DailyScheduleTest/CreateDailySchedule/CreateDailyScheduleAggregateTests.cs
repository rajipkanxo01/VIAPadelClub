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
        var data = dailySchedule.Data;

        // Assert
        Assert.NotNull(data.scheduleId);
        Assert.Equal(ScheduleStatus.Draft, data.status);
        Assert.Empty(data.listOfAvailableCourts);
        Assert.Equal(new TimeSpan(15, 0, 0), data.availableFrom);
        Assert.Equal(new TimeSpan(22, 0, 0), data.availableUntil);
        Assert.Equal(DateTime.Today, data.scheduleDate);
    }
}