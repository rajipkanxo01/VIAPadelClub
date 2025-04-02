using VIAPadelClub.Core.Application.CommandDispatching.Commands.Player;
using VIAPadelClub.Core.Tools.OperationResult;
using Xunit;

namespace UnitTests.Features.PlayerTest.ManagerAddsVIPStatus;

public class ManagerAddsVIPStatusCommandTest
{
    [Fact]
    public void ShouldSucceed_WhenValidGuidIsProvided()
    {
        // Arrange
        var validPlayerId = "123456@via.dk";

        // Act
        var result = ChangePlayerToVipStatusCommand.Create(validPlayerId);

        // Assert
        Assert.True(result.Success);
    }

    [Fact]
    public void ShouldFail_WhenInvalidGuidIsProvided()
    {
        // Arrange
        var invalidPlayerId = "12388888456@via.dk";

        // Act
        var result = ChangePlayerToVipStatusCommand.Create(invalidPlayerId);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(PlayerError.InvalidEmailFormat()._message, result.ErrorMessage);
    }
}