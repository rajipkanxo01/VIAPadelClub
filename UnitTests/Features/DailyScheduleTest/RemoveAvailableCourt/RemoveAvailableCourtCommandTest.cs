using VIAPadelClub.Core.Application.CommandDispatching.Commands.DailySchedule;
using VIAPadelClub.Core.Tools.OperationResult;
using Xunit;

namespace UnitTests.Features.DailyScheduleTest.RemoveAvailableCourt;

public class RemoveAvailableCourtCommandTest
{
    [Fact]
    public void Succeed_When_Valid_Command_Is_Provided()
    {
        // Arrange
        string dailyScheduleIdStr = Guid.NewGuid().ToString();
        string courtName = "D1";
        string timeOfRemoval = "16:00";
        
        // Act
        var result = RemoveAvailableCourtCommand.Create(dailyScheduleIdStr, courtName, timeOfRemoval);
        
        // Assert
        Assert.True(result.Success);
    }
    
    [Fact]
    public void Fail_When_Invalid_Input_Is_Provided()
    {
        // Arrange
        string dailyScheduleIdStr = Guid.NewGuid().ToString();
        string courtName = "e1";
        string timeOfRemoval = "13:00";
        
        // Act
        var result = RemoveAvailableCourtCommand.Create(dailyScheduleIdStr, courtName, timeOfRemoval);
        
        // Assert
        Assert.False(result.Success);
        Assert.Equal(DailyScheduleError.InvalidStartingLetter()._message, result.ErrorMessage);
    }
}