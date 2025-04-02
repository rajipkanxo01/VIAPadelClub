using UnitTests.Features.Helpers;
using UnitTests.Features.Helpers.Factory;
using UnitTests.Features.Helpers.Repository;
using VIAPadelClub.Core.Application.CommandDispatching.Commands.Player;
using VIAPadelClub.Core.Application.Features.Booking;
using Xunit;

namespace UnitTests.Features.PlayerTest.RegisterNewAccount;

public class CreatePlayerHandlerTest
{
    [Fact]
    public void ShouldSucceed_WhenValidCommandIsProvided()
    {
        // Arrange
        var fakePlayerRepository = new FakePlayerRepository();
        var fakeEmailUniqueChecker = new FakeUniqueEmailChecker();
        
        var player = PlayerBuilder.CreateValid().WithBlacklisted().BuildAsync().Result.Data;
        fakePlayerRepository.AddAsync(player);
        
        var command = CreatePlayerCommand.Create(player.email.Value, player.fullName.FirstName, player.fullName.LastName, player.url.Value).Data;
        var handler = new CreatePlayerHandler(fakePlayerRepository,fakeEmailUniqueChecker);
        
        // Act
        var result = handler.HandleAsync(command).Result;
        
        // Assert
        Assert.True(result.Success);
    }
}