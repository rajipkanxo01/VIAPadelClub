namespace UnitTests.Features.PlayerTest.ManagerAddVIPStatus;

using VIAPadelClub.Core.Domain.Aggregates.Players;
using Xunit;

public class ManagerAddsVIPStatusPlayerAggregateTest {
    [Fact]
    public void Should_Mark_Player_As_VIP()
    {
        // Arrange
        var player = Player.Register("test@via.dk", "Nescaffe", "Coffee", "http://profile.uri").Data!;
        
        // Act
        var result = player.ChangeToVIPStatus();

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(player.vipMemberShip);
        Assert.True(player.vipMemberShip.IsVIP);
    }
    
    [Fact]
    public void Should_Reject_If_Player_Is_Already_VIP()
    {
        // Arrange
        var player = Player.Register("test@via.dk", "Tim", "Regent", "http://profile.uri").Data!;
        
        // First VIP upgrade
        player.ChangeToVIPStatus();

        // Act
        var result = player.ChangeToVIPStatus(); //We try again

        // Assert
        Assert.False(result.Success); // It shouldn't upgrade again
        Assert.Equal("Player is already a VIP.", result.ErrorMessage);
    }
    
    [Fact]
    public void Should_Reject_If_Player_Is_Quarantined()
    {
        // Arrange
        var player = Player.Register("test@via.dk", "Dairy", "Milk", "http://profile.uri").Data!;
        player.isQuarantined = true;
        
        // Act
        var result = player.ChangeToVIPStatus();

        // Assert
        Assert.False(result.Success);
        Assert.Equal("Quarantined players cannot be elevated to VIP status.", result.ErrorMessage);
    }
    
    [Fact]
    public void Should_Reject_If_Player_Is_Blacklisted()
    {
        // Arrange
        var player = Player.Register("test@via.dk", "Chew", "Gum", "http://profile.uri").Data!;
        player.isBlackListed = true;
        
        // Act
        var result = player.ChangeToVIPStatus();

        // Assert
        Assert.False(result.Success);
        Assert.Equal("Blacklisted players cannot be elevated to VIP status.", result.ErrorMessage);
    }
}