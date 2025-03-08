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
        var newPlayer = CreateNewPlayer();
        var dailySchedules = new List<DailySchedule>();

        // Act
        var result = newPlayer.Blacklist(dailySchedules);

        // Assert
        Assert.True(newPlayer.isBlackListed);
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
        var newPlayer = CreateNewPlayer();
        var dailySchedules = new List<DailySchedule>();


        newPlayer.Quarantine(DateOnly.Parse(startDate), dailySchedules);

        // Act
        var result = newPlayer.Blacklist(dailySchedules);

        // Assert
        Assert.True(result.Success);
        Assert.True(newPlayer.isBlackListed);
        Assert.Null(newPlayer.activeQuarantine);
    }

    [Fact]
    public void Should_Fail_If_Player_Is_Already_Blacklisted()
    {
        // Arrange
        var newPlayer = CreateNewPlayer();
        var dailySchedules = new List<DailySchedule>();

        newPlayer.Blacklist(dailySchedules);

        // Act
        var result = newPlayer.Blacklist(dailySchedules);

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


    private Player CreateNewPlayer()
    {
        var result = Player.Register("111111@via.dk", "Player", "First", "https://player1profile.com");
        return result.Data;
    }
}