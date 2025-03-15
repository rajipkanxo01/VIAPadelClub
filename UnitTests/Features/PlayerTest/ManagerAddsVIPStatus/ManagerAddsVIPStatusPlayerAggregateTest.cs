using UnitTests.Features.Helpers;
using VIAPadelClub.Core.Domain.Aggregates.Players.Values;

namespace UnitTests.Features.PlayerTest.ManagerAddVIPStatus;

using VIAPadelClub.Core.Domain.Aggregates.Players;
using Xunit;

public class ManagerAddsVIPStatusPlayerAggregateTest {
    [Fact]
    public async Task Should_Mark_Player_As_VIP()
    {
        // Arrange
        var emailChecker = new FakeUniqueEmailChecker();
        var email = Email.Create("test@via.dk");
        var fullName = FullName.Create("John", "Doe");
        var profileUri = ProfileUri.Create("http://example.com");
        var player = await Player.Register(email.Data, fullName.Data, profileUri.Data,emailChecker);
        
        // Act
        var result = player.Data.ChangeToVIPStatus();

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(player.Data.vipMemberShip);
        Assert.True(player.Data.vipMemberShip.IsVIP);
    }
    
    [Fact]
    public async Task Should_Reject_If_Player_Is_Already_VIP()
    {
        // Arrange
        var emailChecker = new FakeUniqueEmailChecker();
        var email = Email.Create("test@via.dk");
        var fullName = FullName.Create("John", "Doe");
        var profileUri = ProfileUri.Create("http://example.com");
        var player = await Player.Register(email.Data, fullName.Data, profileUri.Data,emailChecker);
        
        // First VIP upgrade
        player.Data.ChangeToVIPStatus();

        // Act
        var result = player.Data.ChangeToVIPStatus(); //We try again

        // Assert
        Assert.False(result.Success); // It shouldn't upgrade again
        Assert.Equal("Player is already a VIP.", result.ErrorMessage);
    }
    
    [Fact]
    public async Task Should_Reject_If_Player_Is_Quarantined()
    {
        // Arrange
        var emailChecker = new FakeUniqueEmailChecker();
        var email = Email.Create("test@via.dk");
        var fullName = FullName.Create("John", "Doe");
        var profileUri = ProfileUri.Create("http://example.com");
        var player =await Player.Register(email.Data, fullName.Data, profileUri.Data,emailChecker);
        player.Data.isQuarantined = true;
        
        // Act
        var result = player.Data.ChangeToVIPStatus();

        // Assert
        Assert.False(result.Success);
        Assert.Equal("Quarantined players cannot be elevated to VIP status.", result.ErrorMessage);
    }
    
    [Fact]
    public async Task Should_Reject_If_Player_Is_Blacklisted()
    {
        // Arrange
        var emailChecker = new FakeUniqueEmailChecker();
        var email = Email.Create("test@via.dk");
        var fullName = FullName.Create("John", "Doe");
        var profileUri = ProfileUri.Create("http://example.com");
        var player = await Player.Register(email.Data, fullName.Data, profileUri.Data,emailChecker);
        player.Data.isBlackListed = true;
        
        // Act
        var result = player.Data.ChangeToVIPStatus();

        // Assert
        Assert.False(result.Success);
        Assert.Equal("Blacklisted players cannot be elevated to VIP status.", result.ErrorMessage);
    }
}