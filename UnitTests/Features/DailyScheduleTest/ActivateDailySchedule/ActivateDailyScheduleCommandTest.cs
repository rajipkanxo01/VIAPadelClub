using VIAPadelClub.Core.Application.CommandDispatching.Commands.DailySchedule;
using VIAPadelClub.Core.Tools.OperationResult;
using Xunit;

namespace UnitTests.Features.DailyScheduleTest.ActivateDailySchedule;

public class ActivateDailyScheduleCommandTest
{
    [Fact]
    public void Should_Activate_DailySchedule_When_Valid_Input_Is_Provided()
    {
        // Arrange
        string dailyScheduleIdStr = Guid.NewGuid().ToString();
        
        // Act
        var result = ActivateDailyScheduleCommand.Create(dailyScheduleIdStr);
        
        // Assert
        Assert.True(result.Success);
    }
    
    [Fact]
    public void Should_Fail_When_Invalid_DailyScheduleId_Is_Provided()
    {
        // Arrange
        string invalidDailyScheduleIdStr = "invalid-guid";
        
        // Act
        var result = ActivateDailyScheduleCommand.Create(invalidDailyScheduleIdStr);
        
        // Assert
        Assert.False(result.Success);
        Assert.NotNull(result.ErrorMessage);
        Assert.Equal(DailyScheduleError.InvalidScheduleIdFormatWhileParsing()._message, result.ErrorMessage);
    }
}