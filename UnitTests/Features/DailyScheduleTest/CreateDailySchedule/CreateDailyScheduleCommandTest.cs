using VIAPadelClub.Core.Application.CommandDispatching.Commands.DailySchedule;
using Xunit;

namespace UnitTests.Features.DailyScheduleTest.CreateDailySchedule;

public class CreateDailyScheduleCommandTest
{
    [Fact]
    public void Create_ShouldReturnSuccessResult()
    {
        // Act
        var result = CreateDailyScheduleCommand.Create();

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.IsType<CreateDailyScheduleCommand>(result.Data);
    }
}