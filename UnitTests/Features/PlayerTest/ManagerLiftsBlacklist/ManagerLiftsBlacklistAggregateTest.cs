using UnitTests.Features.Helpers;
using UnitTests.Features.Helpers.Repository;
using VIAPadelClub.Core.Domain.Aggregates.DailySchedules;
using VIAPadelClub.Core.Domain.Aggregates.Players;
using VIAPadelClub.Core.Domain.Aggregates.Players.Values;
using Xunit;

namespace UnitTests.Features.PlayerTest.ManagerLiftsBlacklist;

public class ManagerLiftsBlacklistAggregateTest
{
    [Fact]
    public async Task Should_Lift_Blacklist_When_Selected()
    {
        // Arrange
        var emailChecker = new FakeUniqueEmailChecker();
        var email = Email.Create("test@via.dk");
        var fullName = FullName.Create("John", "Doe");
        var profileUri = ProfileUri.Create("http://example.com");
        var player = await Player.Register(email.Data, fullName.Data, profileUri.Data,emailChecker);

        var fakeScheduleFinder = new FakeScheduleFinder();
        var dailySchedules = new List<DailySchedule>();

        player.Data.Blacklist(fakeScheduleFinder);

        // Act
        var result = player.Data.LiftBlacklist();

        // Assert
        Assert.True(result.Success);
        Assert.Empty(result.ErrorMessage);
        Assert.False(player.Data.isBlackListed);
    }

    [Fact]
    public async Task Should_Fail_When_Unblacklisting_A_Player_Who_Is_Not_Blacklisted()
    {
        // Arrange
        var emailChecker = new FakeUniqueEmailChecker();
        var email = Email.Create("test@via.dk");
        var fullName = FullName.Create("John", "Doe");
        var profileUri = ProfileUri.Create("http://example.com");
        var player = await Player.Register(email.Data, fullName.Data, profileUri.Data,emailChecker);
        var dailySchedules = new List<DailySchedule>();

        // Act
        var result = player.Data.LiftBlacklist();

        // Assert
        Assert.False(result.Success);
        Assert.Equal("Player is not blacklisted.", result.ErrorMessage);
    }
}