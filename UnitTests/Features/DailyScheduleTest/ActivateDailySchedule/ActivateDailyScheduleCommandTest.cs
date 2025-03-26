using VIAPadelClub.Core.Application.CommandDispatching.Commands.DailySchedule;
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
}