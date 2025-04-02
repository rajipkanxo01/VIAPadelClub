using System;
using VIAPadelClub.Core.Application.CommandDispatching.Commands.DailySchedule;
using Xunit;

namespace UnitTests.Features.DailyScheduleTest.DeleteSchedule;

public class DeleteDailyScheduleCommandTest
{
    [Fact]
    public void ShouldSucceed_WhenValidGuidIsProvided()
    {
        // Arrange
        string validId = Guid.NewGuid().ToString();

        // Act
        var result = DeleteDailyScheduleCommand.Create(validId);

        // Assert
        Assert.True(result.Success);
    }

    [Fact]
    public void ShouldFail_WhenInvalidGuidIsProvided()
    {
        // Arrange
        string invalidId = "not-a-guid";

        // Act
        var result = DeleteDailyScheduleCommand.Create(invalidId);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("Invalid schedule ID format.", result.ErrorMessage);
    }
}
