using VIAPadelClub.Core.Application.CommandDispatching.Commands.Player;
using Xunit;

namespace UnitTests.Features.PlayerTest.PlayerQuarantineTest;

public class PlayerQuarantineCommandTest
{
    [Fact]
    public void ShouldSucceed_WhenValidEmailAndDateAreProvided()
    {
        // Arrange
        var email = "test@via.dk";
        var startDate = DateOnly.FromDateTime(DateTime.Today.AddDays(1)).ToString();

        // Act
        var result = QuarantinesPlayerCommand.Create(email, startDate);

        // Assert
        Assert.True(result.Success);
    }

    [Fact]
    public void ShouldFail_WhenInvalidEmailIsProvided()
    {
        // Arrange
        var email = "not-an-email";
        var startDate = DateOnly.FromDateTime(DateTime.Today.AddDays(1)).ToString();

        // Act
        var result = QuarantinesPlayerCommand.Create(email, startDate);

        // Assert
        Assert.False(result.Success);
    }

    [Fact]
    public void ShouldFail_WhenInvalidDateIsProvided()
    {
        // Arrange
        var email = "test@via.dk";
        var startDate = "invalid-date";

        // Act
        var result = QuarantinesPlayerCommand.Create(email, startDate);

        // Assert
        Assert.False(result.Success);
    }
}