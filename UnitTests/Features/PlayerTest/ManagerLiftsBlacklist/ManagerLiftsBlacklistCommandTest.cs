using VIAPadelClub.Core.Application.CommandDispatching.Commands.Player;
using VIAPadelClub.Core.Tools.OperationResult;
using Xunit;

namespace UnitTests.Features.PlayerTest.ManagerLiftsBlacklist;

public class ManagerLiftsBlacklistCommandTest
{
    [Fact]
    public void ShouldSuccess_WhenValidPlayerIdProvided()
    {
        // Arrange
        string playerEmail = "123456@via.dk";
        
        // Act
        var blacklistsPlayerCommand = LiftsBlacklistsPlayerCommand.Create(playerEmail);

        // Assert
        Assert.True(blacklistsPlayerCommand.Success);
    }
    
    [Fact]
    public void ShouldFail_WhenInValidPlayerId_Provided()
    {
        // Arrange
        string playerEmail = "12346@via.dk";
        
        // Act
        var blacklistsPlayerCommand = LiftsBlacklistsPlayerCommand.Create(playerEmail);

        // Assert
        Assert.False(blacklistsPlayerCommand.Success);
        Assert.Equal(PlayerError.InvalidEmailFormat()._message, blacklistsPlayerCommand.ErrorMessage);
    }
}