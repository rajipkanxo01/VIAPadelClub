using UnitTests.Features.Helpers;
using UnitTests.Features.Helpers.Factory;
using UnitTests.Features.Helpers.Repository;
using VIAPadelClub.Core.Application.CommandDispatching.Commands.Player;
using VIAPadelClub.Core.Application.Features.Booking;
using VIAPadelClub.Core.Domain.Aggregates.DailySchedules.Values;
using Xunit;

namespace UnitTests.Features.PlayerTest.ManagerBlacklistsPlayer;

public class BlacklistPlayerHandlerTests
{
    [Fact]
    public async Task HandleAsync_ShouldBlacklistPlayer_WhenValid()
    {
        // Arrange
        var player = (await PlayerBuilder.CreateValid().BuildAsync()).Data;
        var playerRepo = new FakePlayerRepository();
        await playerRepo.AddAsync(player);

        var schedule = (DailyScheduleBuilder.CreateValid().BuildAsync()).Data;
        var scheduleId = ScheduleId.FromGuid(schedule.ScheduleId.Value).Value;
        var scheduleRepo = new FakeDailyScheduleRepository();
        await scheduleRepo.AddAsync(schedule);

        var fakeScheduleFinder = new FakeScheduleFinder(scheduleRepo);

        var handler = new BlacklistsPlayerHandler(playerRepo, fakeScheduleFinder);

        var command = BlacklistsPlayerCommand.Create(scheduleId.ToString(), player.email.Value).Data;

        // Act
        var result = await handler.HandleAsync(command);

        // Assert
        Assert.True(result.Success);
        Assert.True(player.isBlackListed); 
    }

    [Fact]
    public async Task HandleAsync_ShouldFail_WhenPlayerIsAlreadyBlacklisted()
    {
        // Arrange
        var player = (await PlayerBuilder.CreateValid().BuildAsync()).Data;
        var playerRepo = new FakePlayerRepository();
        await playerRepo.AddAsync(player);

        var schedule = (DailyScheduleBuilder.CreateValid().BuildAsync()).Data;
        var scheduleId = ScheduleId.FromGuid(schedule.ScheduleId.Value).Value;
        var scheduleRepo = new FakeDailyScheduleRepository();
        await scheduleRepo.AddAsync(schedule);

        var fakeScheduleFinder = new FakeScheduleFinder(scheduleRepo);

        var handler = new BlacklistsPlayerHandler(playerRepo, fakeScheduleFinder);

        var command = BlacklistsPlayerCommand.Create(scheduleId.ToString(), player.email.Value).Data;

        // first blacklist
        await handler.HandleAsync(command);
        
        // Act
        var result = await handler.HandleAsync(command);
        
        // Assert
        Assert.False(result.Success);
    }
}