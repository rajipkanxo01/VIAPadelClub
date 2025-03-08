using VIAPadelClub.Core.Domain.Aggregates.Players;
using Xunit;

namespace UnitTests.Features.PlayerTest.ManagerBlacklistsPlayer;

public class ManagerBlacklistPlayerAggregateTest
{
    [Fact]
    public void Should_Blacklist_Player_When_Selected()
    {
        // Arrange
        var newPlayer = CreateNewPlayer();

        // Act
        var result = newPlayer.Blacklist();

        // Assert
        Assert.True(newPlayer.isBlackListed);
        Assert.True(result.Success);
    }

    // this test fails for now because quarantine player is not implemented yet!!
    // [Fact]
    /*public void Should_Remove_Quarantine_When_Player_Is_Blacklisted()
    {
        // Arrange
        var newPlayer = CreateNewPlayer();
        
        newPlayer.Quarantine(DateTime.Now, TimeSpan.FromDays(3));

        // Act
        var result = newPlayer.Blacklist();

        // Assert
        Assert.True(result.Success);
        Assert.True(newPlayer.isBlackListed);
        Assert.Null(newPlayer.quarantine);
    }*/

    [Fact]
    public void Should_Fail_If_Player_Is_Already_Blacklisted()
    {
        // Arrange
        var newPlayer = CreateNewPlayer();
        newPlayer.Blacklist();

        // Act
        var result = newPlayer.Blacklist();

        // Assert
        Assert.False(result.Success);
        Assert.Equal("Player Already Blacklisted.", result.ErrorMessage);
    }

    [Fact]
    public void Should_Cancel_Booked_Courts_When_Player_Is_Blacklisted()
    {
        // need to implement this after booking is done!!
    }


    private Player CreateNewPlayer()
    {
        var result = Player.Register("111111@via.dk", "Player", "First", "https://player1profile.com");
        return result.Data;
    }
}