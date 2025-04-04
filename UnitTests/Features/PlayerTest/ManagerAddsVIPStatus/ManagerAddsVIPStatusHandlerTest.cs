using UnitTests.Features.Helpers;
using UnitTests.Features.Helpers.Factory;
using UnitTests.Features.Helpers.Repository;
using VIAPadelClub.Core.Application.CommandDispatching.Commands.Player;
using VIAPadelClub.Core.Application.Features.Booking;
using VIAPadelClub.Core.Tools.OperationResult;
using Xunit;

namespace UnitTests.Features.PlayerTest.ManagerAddsVIPStatus;

public class ManagerAddsVIPStatusHandlerTest
{
    [Fact]
    public async Task HandleAsync_ShouldAddVIPStatus_WhenValid()
    {
        // Arrange
        var player = (await PlayerBuilder.CreateValid().BuildAsync()).Data;
        var playerRepo = new FakePlayerRepository();
        await playerRepo.AddAsync(player);

        var unitOfWork = new FakeUnitOfWork();
        var handler = new ChangePlayerToVipCommandHandler(playerRepo);

        var command = ChangePlayerToVipStatusCommand.Create(player.email.Value).Data;

        // Act
        var result = await handler.HandleAsync(command);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(player.vipMemberShip);
        Assert.True(unitOfWork.SaveChangesCalled);
    }
    
    [Fact]
    public async Task HandleAsync_ShouldFail_WhenPlayerIsAlreadyVIP()
    {
        // Arrange
        var player = (await PlayerBuilder.CreateValid().BuildAsync()).Data;
        player.ChangeToVIPStatus(); // Changed to VIP

        var playerRepo = new FakePlayerRepository();
        await playerRepo.AddAsync(player);

        var handler = new ChangePlayerToVipCommandHandler(playerRepo);

        var command = ChangePlayerToVipStatusCommand.Create(player.email.Value).Data;

        // Act - try again
        var result = await handler.HandleAsync(command);

        // Assert
        Assert.False(result.Success);
    }
}