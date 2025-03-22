using VIAPadelClub.Core.Application.CommandDispatching.Commands.Booking;
using VIAPadelClub.Core.Tools.OperationResult;
using Xunit;

namespace UnitTests.Features.PlayerTest.ManagerBlacklistsPlayer;

public class ManagerBlacklistsPlayerCommandTest
{
    [Fact]
    public void Create_ShouldReturnSuccess_WhenValidIdsAreGiven()
    {
        // Arrange
        var validScheduleId = Guid.NewGuid().ToString();
        var validPlayerId = Guid.NewGuid().ToString();

        // Act
        var result = BlacklistsPlayerCommand.Create(validScheduleId, validPlayerId);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.IsType<BlacklistsPlayerCommand>(result.Data);
    }
    
    [Fact]
    public void Create_ShouldReturnFailure_WhenInvalidPlayerIdIsGiven()
    {
        // Arrange
        var validScheduleId = Guid.NewGuid().ToString();
        var invalidPlayerId = "not-a-guid";

        // Act
        var result = BlacklistsPlayerCommand.Create(validScheduleId, invalidPlayerId);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(ErrorMessage.InvalidPlayerIdFormatWhileParsing()._message, result.ErrorMessage);
    }
}