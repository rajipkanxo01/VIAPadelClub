using VIAPadelClub.Core.Domain.Aggregates.DailySchedules;
using VIAPadelClub.Core.Domain.Aggregates.Players;
using Xunit;

namespace UnitTests.Features.PlayerTest.ManagerBlacklistsPlayer;

public class ManagerBlacklistPlayerAggregateTest
{
    [Fact]
    public void Should_Blacklist_Player_When_Selected()
    {
        // Arrange
        var player = Player.Register("111111@via.dk", "Player", "First", "https://player1profile.com").Data;
        var dailySchedules = new List<DailySchedule>();

        // Act
        var result = player.Blacklist(dailySchedules);

        // Assert
        Assert.True(player.isBlackListed);
        Assert.True(result.Success);
    }

    // this test fails for now because quarantine player is not implemented yet!!
    [Theory]
    [InlineData("2025-03-15")]
    [InlineData("2025-04-22")]
    [InlineData("2025-04-26")]
    public void Should_Remove_Quarantine_When_Player_Is_Blacklisted(string startDate)
    {
        // Arrange
        var player = Player.Register("111111@via.dk", "Player", "First", "https://player1profile.com").Data;
        var dailySchedules = new List<DailySchedule>();


        player.Quarantine(DateOnly.Parse(startDate), dailySchedules);

        // Act
        var result = player.Blacklist(dailySchedules);

        // Assert
        Assert.True(result.Success);
        Assert.True(player.isBlackListed);
        Assert.Null(player.activeQuarantine);
    }

    [Fact]
    public void Should_Fail_If_Player_Is_Already_Blacklisted()
    {
        // Arrange
        var player = Player.Register("111111@via.dk", "Player", "First", "https://player1profile.com").Data;
        var dailySchedules = new List<DailySchedule>();

        player.Blacklist(dailySchedules);

        // Act
        var result = player.Blacklist(dailySchedules);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("Player Already Blacklisted. Cannot blacklist same player twice!!", result.ErrorMessage);
    }

    [Theory]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(0)]
    public void Should_Cancel_Booked_Courts_When_Player_Is_Blacklisted(int numberOfBookings)
    {
        //TODO: need to implement this after booking is done!!
    }
    
}