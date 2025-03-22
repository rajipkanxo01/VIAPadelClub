﻿using UnitTests.Features.Helpers;
using VIAPadelClub.Core.Domain.Aggregates.DailySchedules;
using VIAPadelClub.Core.Domain.Aggregates.Players;
using VIAPadelClub.Core.Domain.Aggregates.Players.Values;
using VIAPadelClub.Core.Tools.OperationResult;
using Xunit;

namespace UnitTests.Features.PlayerTest.ManagerBlacklistsPlayer;

public class ManagerBlacklistPlayerAggregateTest
{
    [Fact]
    public async Task Should_Blacklist_Player_When_Selected()
    {
        // Arrange
        var emailChecker = new FakeUniqueEmailChecker();
        var email = Email.Create("test@via.dk");
        var fullName = FullName.Create("John", "Doe");
        var profileUri = ProfileUri.Create("http://example.com");
        var player = await Player.Register(email.Data, fullName.Data, profileUri.Data,emailChecker);
        var dailySchedules = new List<DailySchedule>();

        var fakeScheduleFinder = new FakeScheduleFinder();

        // Act
        var result = player.Data.Blacklist(fakeScheduleFinder);

        // Assert
        Assert.True(player.Data.isBlackListed);
        Assert.True(result.Success);
    }

    [Theory]
    [InlineData("2025-03-15")]
    [InlineData("2025-04-22")]
    [InlineData("2025-04-26")]
    public async Task Should_Remove_Quarantine_When_Player_Is_Blacklisted(string startDate)
    {
        // Arrange
        var emailChecker = new FakeUniqueEmailChecker();
        var email = Email.Create("test@via.dk");
        var fullName = FullName.Create("John", "Doe");
        var profileUri = ProfileUri.Create("http://example.com");
        var player = await Player.Register(email.Data, fullName.Data, profileUri.Data,emailChecker);
        var dailySchedules = new List<DailySchedule>();

        var fakeScheduleFinder = new FakeScheduleFinder();

        player.Data.Quarantine(DateOnly.Parse(startDate), dailySchedules);

        // Act
        var result = player.Data.Blacklist(fakeScheduleFinder);

        // Assert
        Assert.True(result.Success);
        Assert.True(player.Data.isBlackListed);
        Assert.Null(player.Data.activeQuarantine);
    }

    [Fact]
    public async Task Should_Fail_If_Player_Is_Already_Blacklisted()
    {
        // Arrange
        var emailChecker = new FakeUniqueEmailChecker();
        var email = Email.Create("test@via.dk");
        var fullName = FullName.Create("John", "Doe");
        var profileUri = ProfileUri.Create("http://example.com");
        var player = await Player.Register(email.Data, fullName.Data, profileUri.Data,emailChecker);
        
        var dailySchedules = new List<DailySchedule>();
        var fakeScheduleFinder = new FakeScheduleFinder();
        
        player.Data.Blacklist(fakeScheduleFinder);

        // Act
        var result = player.Data.Blacklist(fakeScheduleFinder);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(ErrorMessage.PlayerAlreadyBlacklisted()._message, result.ErrorMessage);
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