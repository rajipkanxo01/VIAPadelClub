using UnitTests.Features.Helpers;
using UnitTests.Features.Helpers.Factory;
using VIAPadelClub.Core.Domain.Aggregates.Players;
using VIAPadelClub.Core.Domain.Aggregates.Players.Values;
using Xunit;

namespace UnitTests.Features.PlayerTest.ManagerAddsVIPStatus;

public class ManagerAddsVipStatusPlayerAggregateTest {
    [Fact]
    public async Task Should_Mark_Player_As_VIP()
    {
        // Arrange
        var player = (await PlayerBuilder.CreateValid().BuildAsync()).Data;
        
        // Act
        var result = player.ChangeToVIPStatus();

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(player.vipMemberShip);
        Assert.True(player.vipMemberShip.IsVIP);
    }
    
    [Fact]
    public async Task Should_Reject_If_Player_Is_Already_VIP()
    {
        // Arrange
        var player = (await PlayerBuilder.CreateValid().BuildAsync()).Data;
        
        // First VIP upgrade
        player.ChangeToVIPStatus();

        // Act
        var result = player.ChangeToVIPStatus(); //We try again

        // Assert
        Assert.False(result.Success); // It shouldn't upgrade again
        Assert.Equal("Player is already a VIP.", result.ErrorMessage);
    }
    
    [Fact]
    public async Task Should_Reject_If_Player_Is_Quarantined()
    {
        // Arrange
        var player = (await PlayerBuilder.CreateValid().BuildAsync()).Data;
        player.isQuarantined = true;
        
        // Act
        var result = player.ChangeToVIPStatus();

        // Assert
        Assert.False(result.Success);
        Assert.Equal("Quarantined players cannot be elevated to VIP status.", result.ErrorMessage);
    }
    
    [Fact]
    public async Task Should_Reject_If_Player_Is_Blacklisted()
    {
        // Arrange
        var player = (await PlayerBuilder.CreateValid().BuildAsync()).Data;
        player.isBlackListed = true;
        
        // Act
        var result = player.ChangeToVIPStatus();

        // Assert
        Assert.False(result.Success);
        Assert.Equal("Blacklisted players cannot be elevated to VIP status.", result.ErrorMessage);
    }
}