using VIAPadelClub.Core.Domain.Aggregates.Players;
using VIAPadelClub.Core.Domain.Aggregates.Players.Values;
using VIAPadelClub.Core.Tools.OperationResult;
using Xunit;

namespace UnitTests.Features.PlayerTest.RegisterNewAccount;

public class RegisterNewAccountAggregateTest
{
    [Theory]
    [InlineData("john@via.dk", "John", "Doe", "https://profile.com/john", true)]
    [InlineData("invalidemail.com", "John", "Doe", "https://profile.com/john", false)]
    [InlineData("john@via.dk", "J", "Doe", "https://profile.com/john", false)]
    [InlineData("john@via.dk", "John", "Doe", "", false)]
    public void Should_Register_User_With_Valid_Fields(string email, string firstName, string lastName, string profileUri, bool expectedSuccess)
    {
        var result = Player.Register(email, firstName, lastName, profileUri);
        Assert.Equal(expectedSuccess, result.Success);
    }
    
    [Theory]
    [InlineData("test@gmail.com")]
    [InlineData("123@sth.dk")]
    public void Should_Return_Failure_With_Invalid_Email_Domain(string email)
    {
        var result = Email.Create(email);
        Assert.False(result.Success);
        Assert.NotNull(result.ErrorMessage);
        Assert.Contains(ErrorMessage.EmailMustEndWithViaDk()._message, result.ErrorMessage);
    }
    
    [Theory]
    [InlineData("@via.com")]
    [InlineData("123@via,com")]
    public void Should_Return_Failure_With_Incorrect_Email_Format(string email)
    {
        var result = Email.Create(email);
        Assert.False(result.Success);
        Assert.NotNull(result.ErrorMessage);
        Assert.Contains(ErrorMessage.InvalidEmailFormat()._message, result.ErrorMessage);
    }
    
    [Theory]
    [InlineData("")]
    public void Should_Return_Failure_With_Empty_Email(string email)
    {
        var result = Email.Create(email);
        Assert.False(result.Success);
        Assert.NotNull(result.ErrorMessage);
        Assert.Contains(ErrorMessage.EmailCannotBeEmpty()._message, result.ErrorMessage);
    }
    
    [Theory]
    [InlineData("abc@via.dk")]
    public void Should_Return_Success_With_Valid_Email_Format(string email)
    {
        var result = Email.Create(email);
        Assert.True(result.Success);
        Assert.Equal(email.ToLower(),result.Data!.Value);
    }
    
    [Theory]
    [InlineData("John", "Doe")]
    [InlineData("Alice", "Smith")]
    public void Should_Return_Success_With_Valid_FullName(string firstName, string lastName)
    {
        var result = FullName.Create(firstName, lastName);
        Assert.True(result.Success);
        Assert.Equal(firstName, result.Data!.FirstName, ignoreCase: true);
        Assert.Equal(lastName, result.Data!.LastName, ignoreCase: true);
    }
    
    [Theory]
    [InlineData("J", "Doe")]
    [InlineData("John", "D")]
    [InlineData("John123", "Doe")]
    [InlineData("John", "Doe!")]
    public void Should_Return_Failure_With_Invalid_FullName(string firstName, string lastName)
    {
        var result = FullName.Create(firstName, lastName);
        Assert.False(result.Success);
        Assert.NotNull(result.ErrorMessage);
       }
    
    [Fact]
    public void Should_Return_Success_With_Valid_URL()
    {
        var result = ProfileUri.Create("https://example.com");
        Assert.True(result.Success);
        Assert.Equal("https://example.com", result.Data!.Value);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void Should_Return_Failure_With_Invalid_URL(string url)
    {
        var result = ProfileUri.Create(url);
        Assert.False(result.Success);
        Assert.NotNull(result.ErrorMessage);
        Assert.Contains(ErrorMessage.InvalidProfileUri()._message, result.ErrorMessage);
    }
}