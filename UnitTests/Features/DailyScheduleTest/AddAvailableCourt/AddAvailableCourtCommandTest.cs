using VIAPadelClub.Core.Application.CommandDispatching.Commands.DailySchedule;
using VIAPadelClub.Core.Tools.OperationResult;
using Xunit;

namespace UnitTests.Features.DailyScheduleTest.AddAvailableCourt;

public class AddAvailableCourtCommandTest
{
    [Fact]
    public void Success_When_Valid_Input_Is_Provided()
    {
        // Arrange
        string dailyScheduleIdStr = Guid.NewGuid().ToString();
        string courtName = "D1";
        
        // Act
        var result = AddAvailableCourtCommand.Create(dailyScheduleIdStr, courtName);
        
        // Assert
        Assert.True(result.Success);
    }
    
    [Fact]
    public void Fail_When_Invalid_Input_Is_Not_Provided()
    {
        // Arrange
        string dailyScheduleIdStr = Guid.NewGuid().ToString();
        string courtName = "e1";
        
        // Act
        var result = AddAvailableCourtCommand.Create(dailyScheduleIdStr, courtName);
        
        // Assert
        Assert.False(result.Success);
        Assert.Equal(DailyScheduleError.InvalidStartingLetter()._message, result.ErrorMessage);
    }
}