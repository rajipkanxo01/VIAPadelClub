using UnitTests.Features.Helpers.Factory;
using VIAPadelClub.Core.Domain.Aggregates.DailySchedules;
using VIAPadelClub.Core.Tools.OperationResult;
using Xunit;

namespace UnitTests.Features.PlayerTest.ManagerLiftsBlacklist;

public class ManagerLiftsBlacklistAggregateTest
{
    [Fact]
    public async Task Should_Lift_Blacklist_When_Selected()
    {
        // Arrange
        var player = (await PlayerBuilder.CreateValid().BuildAsync()).Data;

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
    public async Task Should_Fail_When_Unblacklisting_A_Player_Who_Is_Not_Blacklisted()
    {
        // Arrange
        var player = (await PlayerBuilder.CreateValid().BuildAsync()).Data;
        var dailySchedules = new List<DailySchedule>();

        // Act
        var result = player.LiftBlacklist();

        // Assert
        Assert.False(result.Success);
        Assert.Equal(DailyScheduleError.PlayerIsNotBlacklisted()._message, result.ErrorMessage);
    }
}