using VIAPadelClub.Core.Application.CommandDispatching.Commands.Player;
using VIAPadelClub.Core.Tools.OperationResult;
using Xunit;

namespace UnitTests.Features.PlayerTest.RegisterNewAccount;

public class CreatePlayerCommandTest
{
    [Fact]
    public void Succeed_WhenValidCommandIsProvided()
    {
        // Arrange
        string email = "test@via.dk";
        string firstName = "Test";
        string lastName = "Student";
        string profileUrl= "https://via.dk";
        
        // Act
        var result = CreatePlayerCommand.Create(email, firstName, lastName, profileUrl);
        
        // Assert
        Assert.True(result.Success);
    }
    
    [Fact]
    public void Fail_WhenInvalidEmailIsProvided()
    {
        // Arrange
        string email = "student@via.dk";
        string firstName = "Test";
        string lastName = "Student";
        string profileUrl= "https://via.dk";
        
        // Act
        var result = CreatePlayerCommand.Create(email, firstName, lastName, profileUrl);
        
        // Assert
        Assert.False(result.Success);
        Assert.Equal(PlayerError.InvalidEmailFormat()._message, result.ErrorMessage);
    }
}
