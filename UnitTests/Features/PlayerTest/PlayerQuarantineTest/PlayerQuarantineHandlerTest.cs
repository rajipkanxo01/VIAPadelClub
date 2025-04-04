using UnitTests.Features.Helpers;
using UnitTests.Features.Helpers.Factory;
using UnitTests.Features.Helpers.Repository;
using VIAPadelClub.Core.Application.CommandDispatching.Commands.Player;
using VIAPadelClub.Core.Application.Features.Booking;
using VIAPadelClub.Core.Domain.Aggregates.DailySchedules;
using Xunit;

namespace UnitTests.Features.PlayerTest.PlayerQuarantineTest;

public class PlayerQuarantineHandlerTest
{
    [Fact]
    public async Task ShouldQuarantinePlayer_WhenValid()
    {
        // Arrange
        var player = (await PlayerBuilder.CreateValid().BuildAsync()).Data;
        var schedule = DailyScheduleBuilder.CreateValid().BuildAsync().Data;

        var playerRepo = new FakePlayerRepository();
        await playerRepo.AddAsync(player);

        var scheduleRepo = new FakeDailyScheduleRepository();
        await scheduleRepo.AddAsync(schedule);

        var unitOfWork = new FakeUnitOfWork();
        var handler = new QuarantinesPlayerCommandHandler(playerRepo, scheduleRepo);

        var command = QuarantinesPlayerCommand.Create(player.email.Value, DateOnly.FromDateTime(DateTime.Today.AddDays(1)).ToString()).Data;

        // Act
        var result = await handler.HandleAsync(command);

        // Assert
        Assert.True(result.Success);
        Assert.True(player.isQuarantined);
        Assert.True(unitOfWork.SaveChangesCalled);
    }
    
    [Fact]
    public async Task ShouldFailToQuarantine_WhenPlayerIsBlacklisted()
    {
        // Arrange
        var player = (await PlayerBuilder.CreateValid().BuildAsync()).Data;
        player.isBlackListed = true;

        var schedule = DailyScheduleBuilder.CreateValid().BuildAsync().Data;

        var playerRepo = new FakePlayerRepository();
        await playerRepo.AddAsync(player);

        var scheduleRepo = new FakeDailyScheduleRepository();
        await scheduleRepo.AddAsync(schedule);

        var handler = new QuarantinesPlayerCommandHandler(playerRepo, scheduleRepo);

        var command = QuarantinesPlayerCommand.Create(player.email.Value, DateOnly.FromDateTime(DateTime.Today.AddDays(1)).ToString()).Data;

        // Act
        var result = await handler.HandleAsync(command);

        // Assert
        Assert.False(result.Success);
        Assert.False(player.isQuarantined);
        Assert.Empty(player.quarantines);
    }
}