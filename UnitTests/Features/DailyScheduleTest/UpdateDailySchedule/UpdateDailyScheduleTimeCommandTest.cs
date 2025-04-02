using VIAPadelClub.Core.Application.CommandDispatching.Commands.DailySchedule;
using VIAPadelClub.Core.Tools.OperationResult;
using Xunit;

namespace UnitTests.Features.DailyScheduleTest.UpdateDailySchedule;

public class UpdateDailyScheduleTimeCommandTest
{
    [Fact]
    public void ShouldSucceed_WhenValidInputsAreProvided()
    {
        // Arrange
        string scheduleId = Guid.NewGuid().ToString();
        string date = DateOnly.FromDateTime(DateTime.Today.AddDays(1)).ToString(); // tomorrow
        string startTime = "10:00";
        string endTime = "12:00";

        // Act
        var result = UpdateDailyScheduleTimeCommand.Create(scheduleId, date, startTime, endTime);

        // Assert
        Assert.True(result.Success);
    }

    [Fact]
    public void ShouldFail_WhenEndTimeIsInvalid()
    {
        // Arrange
        string scheduleId = Guid.NewGuid().ToString();
        string date = DateOnly.FromDateTime(DateTime.Today.AddDays(1)).ToString();
        string startTime = "10:00";
        string endTime = ":00"; // Invalid

        // Act
        var result = UpdateDailyScheduleTimeCommand.Create(scheduleId, date, startTime, endTime);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(DailyScheduleError.InvalidTimeformatWhileParsing()._message, result.ErrorMessage);
    }
}