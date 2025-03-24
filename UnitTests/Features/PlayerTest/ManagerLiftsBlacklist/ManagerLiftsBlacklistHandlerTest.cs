using UnitTests.Features.Helpers.Factory;
using UnitTests.Features.Helpers.Repository;
using VIAPadelClub.Core.Application.CommandDispatching.Commands.Player;
using VIAPadelClub.Core.Application.Features;
using VIAPadelClub.Core.Application.Features.Booking;
using VIAPadelClub.Core.Tools.OperationResult;
using Xunit;

namespace UnitTests.Features.PlayerTest.ManagerLiftsBlacklist;

public class ManagerLiftsBlacklistHandlerTest
{
    [Fact]
    public void ShouldSuccess_WhenValidCommandIsProvided()
    {
        // Arrange
        var fakePlayerRepository = new FakePlayerRepository();

        var player = PlayerBuilder.CreateValid().WithBlacklisted().BuildAsync().Result.Data;
        fakePlayerRepository.AddAsync(player);
        
        var liftsBlacklistsPlayerCommand = LiftsBlacklistsPlayerCommand.Create(player.Email.Value);

        var playerHandler = new LiftsBlacklistsPlayerHandler(fakePlayerRepository);
        
        // Act
        var result = playerHandler.HandleAsync(liftsBlacklistsPlayerCommand.Data).Result;

        // Assert
        Assert.True(result.Success);
    }

    [Fact]
    public void ShouldFail_WhenPlayerIsNotBlacklisted()
    {
        // Arrange
        var fakePlayerRepository = new FakePlayerRepository();
        
        var player = PlayerBuilder.CreateValid().BuildAsync().Result.Data;
        fakePlayerRepository.AddAsync(player);
        
        var liftsBlacklistsPlayerCommand = LiftsBlacklistsPlayerCommand.Create(player.Email.Value);
        
        var playerHandler = new LiftsBlacklistsPlayerHandler(fakePlayerRepository);
        
        // Act
        var result = playerHandler.HandleAsync(liftsBlacklistsPlayerCommand.Data).Result;

        // Assert
        Assert.False(result.Success);
        Assert.Equal(DailyScheduleError.PlayerIsNotBlacklisted()._message, result.ErrorMessage);
    }
}