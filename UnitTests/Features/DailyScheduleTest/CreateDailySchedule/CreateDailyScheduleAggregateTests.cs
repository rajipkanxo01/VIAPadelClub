using UnitTests.Features.Helpers;
using VIAPadelClub.Core.Domain.Aggregates.DailySchedules;
using VIAPadelClub.Core.Domain.Aggregates.DailySchedules.Values;
using Xunit;

namespace UnitTests.Features.DailyScheduleTest.CreateDailySchedule;

public class CreateDailyScheduleAggregateTests
{
    [Fact]
    public void Should_Create_DailySchedule_With_Default_Values()
    {
        // Arrange
        var scheduleId = ScheduleId.FromGuid(Guid.NewGuid());

        var fakeDateProvider = new FakeDateProvider(DateOnly.FromDateTime(DateTime.Today));
        
        // Act
        var dailySchedule = DailySchedule.CreateSchedule(fakeDateProvider, scheduleId);
        var data = dailySchedule.Data;

        // Assert
        Assert.NotNull(data.scheduleId);
        Assert.Equal(ScheduleStatus.Draft, data.status);
        Assert.Empty(data.listOfAvailableCourts);
        Assert.Equal(new TimeOnly(15, 0, 0), data.availableFrom);
        Assert.Equal(new TimeOnly(22, 0, 0), data.availableUntil);
        Assert.Equal(DateOnly.FromDateTime(DateTime.Today), data.scheduleDate);
    }
}