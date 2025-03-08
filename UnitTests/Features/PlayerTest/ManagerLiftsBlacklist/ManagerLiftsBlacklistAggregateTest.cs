using VIAPadelClub.Core.Domain.Aggregates.DailySchedules;
using VIAPadelClub.Core.Domain.Aggregates.Players;
using Xunit;

namespace UnitTests.Features.PlayerTest.ManagerLiftsBlacklist;

public class ManagerLiftsBlacklistAggregateTest
{
    [Fact]
    public void Should_Lift_Blacklist_When_Selected()
    {
        // Arrange
        var player = Player.Register("111111@via.dk", "Player", "First", "https://player1profile.com").Data;
        var dailySchedules = new List<DailySchedule>();

        player.Blacklist(dailySchedules);

        // Act
        var result = player.LiftBlacklist();

        // Assert
        Assert.True(result.Success);
        Assert.Empty(result.ErrorMessage);
        Assert.False(player.isBlackListed);
    }

    [Fact]
    public void Should_Fail_When_Unblacklisting_A_Player_Who_Is_Not_Blacklisted()
    {
        // Arrange
        var player = Player.Register("111111@via.dk", "Player", "First", "https://player1profile.com").Data;
        var dailySchedules = new List<DailySchedule>();

        // Act
        var result = player.LiftBlacklist();

        // Assert
        Assert.False(result.Success);
        Assert.Equal("Player is not blacklisted.", result.ErrorMessage);
    }
}