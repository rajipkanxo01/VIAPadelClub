using UnitTests.Features.Helpers;
using UnitTests.Features.Helpers.Factory;
using UnitTests.Features.Helpers.Repository;
using VIAPadelClub.Core.Application.CommandDispatching.Commands.DailySchedule;
using VIAPadelClub.Core.Application.Features.Daily_Schedule;
using Xunit;

namespace UnitTests.Features.DailyScheduleTest.UpdateDailySchedule;

public class UpdateDailyScheduleTimeHandlerTest
{
    [Fact]
    public void ShouldSucceed_WhenValidCommandIsProvided()
    {
        // Arrange
        var dailySchedule = DailyScheduleBuilder.CreateValid().BuildAsync().Data;
        var command = UpdateDailyScheduleTimeCommand.Create(
            dailySchedule.Id.ToString(),
            DateOnly.FromDateTime(DateTime.Today.AddDays(1)).ToString(),
            "10:00", "12:00"
        ).Data;

        var repo = new FakeDailyScheduleRepository();
        repo.AddAsync(dailySchedule);

        var handler = new UpdateDailyScheduleTimeHandler(repo,
            new FakeDateProvider(DateOnly.FromDateTime(DateTime.Today)));

        // Act
        var result = handler.HandleAsync(command).Result;

        // Assert
        Assert.True(result.Success);
    }

    [Fact]
    public void ShouldFail_WhenCommandUsesPastDate()
    {
        // Arrange
        var dailySchedule = DailyScheduleBuilder.CreateValid().BuildAsync().Data;
        var command = UpdateDailyScheduleTimeCommand.Create(
            dailySchedule.Id.ToString(),
            DateOnly.FromDateTime(DateTime.Today.AddDays(-1)).ToString(), // past
            "10:00", "12:00"
        ).Data;

        var repo = new FakeDailyScheduleRepository();
        repo.AddAsync(dailySchedule);

        var handler = new UpdateDailyScheduleTimeHandler(repo,new FakeDateProvider(DateOnly.FromDateTime(DateTime.Today)));

        // Act
        var result = handler.HandleAsync(command).Result;

        // Assert
        Assert.False(result.Success);
    }
}