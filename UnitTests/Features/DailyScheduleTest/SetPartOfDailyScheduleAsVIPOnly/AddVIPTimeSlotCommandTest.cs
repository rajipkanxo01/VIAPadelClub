using VIAPadelClub.Core.Application.CommandDispatching.Commands.DailySchedule;
using VIAPadelClub.Core.Tools.OperationResult;
using Xunit;

namespace UnitTests.Features.DailyScheduleTest.SetPartOfDailyScheduleAsVIPOnly;

public class AddVipTimeSlotCommandTest
{
    [Fact]
    public void ShouldSucceed_WhenValidInputsAreProvided()
    {
        // Arrange
        string scheduleId = Guid.NewGuid().ToString();
        string startTime = "10:00";
        string endTime = "12:00";

        // Act
        var result = AddVipTimeSlotCommand.Create(scheduleId, startTime, endTime);

        // Assert
        Assert.True(result.Success);
    }
    
    [Fact]
    public void ShouldSucceed_WhenInvalidInputsAreProvided()
    {
        // Arrange
        string scheduleId = Guid.NewGuid().ToString();
        string startTime = "10:00";
        string endTime = ":00";

        // Act
        var result = AddVipTimeSlotCommand.Create(scheduleId, startTime, endTime);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(ErrorMessage.InvalidTimeformatWhileParsing()._message, result.ErrorMessage);
    }
}