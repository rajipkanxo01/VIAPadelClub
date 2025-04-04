using UnitTests.Features.Helpers;
using VIAPadelClub.Core.Domain.Aggregates.Players;
using VIAPadelClub.Core.Domain.Aggregates.Players.Values;
using VIAPadelClub.Core.Tools.OperationResult;
using Xunit;

namespace UnitTests.Features.PlayerTest.RegisterNewAccount;

public class RegisterNewAccountAggregateTest
{
    [Fact]
    public async Task Should_Succeed_When_Input_Are_Valid_For_Registration()
    {
        // Arrange
        var emailChecker = new FakeUniqueEmailChecker();
        emailChecker.AddEmail(Email.Create("tesd@via.dk").Data.Value);
        emailChecker.AddEmail(Email.Create("123456@via.dk").Data.Value);
        var email = Email.Create("test@via.dk");
        var fullName = FullName.Create("John", "Doe");
        var profileUri = ProfileUri.Create("http://example.com");

        // Act
        var result = await Player.Register(email.Data, fullName.Data, profileUri.Data,emailChecker);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Data);
    }

    [Fact]
    public async Task Should_Fail_When_Email_Is_Duplicate()
    {
       //Arrange
         var emailChecker = new FakeUniqueEmailChecker();
         var email = Email.Create("test@via.dk");
         emailChecker.AddEmail(email.Data.Value);
         var fullName = FullName.Create("John", "Doe");
         var profileUri = ProfileUri.Create("http://example.com");
         
         //Act
         var result = await Player.Register(email.Data, fullName.Data, profileUri.Data,emailChecker);
         
         //Assert
         Assert.False(result.Success);
         Assert.NotNull(result.ErrorMessage);
    }

    
    [Theory]
    [InlineData("test@gmail.com")]
    [InlineData("123@sth.dk")]
    public void Should_Return_Failure_With_Invalid_Email_Domain(string email)
    {
        var result = Email.Create(email);
        Assert.False(result.Success);
        Assert.NotNull(result.ErrorMessage);
        Assert.Contains(PlayerError.EmailMustEndWithViaDk()._message, result.ErrorMessage);
    }
    
    [Theory]
    [InlineData("@via.com")]
    [InlineData("123@via,com")]
    [InlineData("123as@via.com")]
    [InlineData("1235678@via.com")]
    [InlineData("ab@via.com")]
    [InlineData("qwesd@via.com")]
    public void Should_Return_Failure_With_Incorrect_Email_Format(string email)
    {
        var result = Email.Create(email);
        Assert.False(result.Success);
        Assert.NotNull(result.ErrorMessage);
        Assert.Contains(PlayerError.InvalidEmailFormat()._message, result.ErrorMessage);
    }
    
    [Theory]
    [InlineData("")]
    public void Should_Return_Failure_With_Empty_Email(string email)
    {
        var result = Email.Create(email);
        Assert.False(result.Success);
        Assert.NotNull(result.ErrorMessage);
        Assert.Contains(PlayerError.EmailCannotBeEmpty()._message, result.ErrorMessage);
    }
    
    [Theory]
    [InlineData("abc@via.dk")]
    [InlineData("abca@via.dk")]
    [InlineData("123456@via.dk")]
    public void Should_Return_Success_With_Valid_Email_Format(string email)
    {
        var result = Email.Create(email);
        Assert.True(result.Success);
        Assert.Equal(email.ToLower(),result.Data.Value);
    }
    
    [Theory]
    [InlineData("John", "Doe")]
    [InlineData("Alice", "Smith")]
    public void Should_Return_Success_With_Valid_FullName(string firstName, string lastName)
    {
        var result = FullName.Create(firstName, lastName);
        Assert.True(result.Success);
        Assert.Equal(firstName, result.Data.FirstName, ignoreCase: true);
        Assert.Equal(lastName, result.Data.LastName, ignoreCase: true);
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
        Assert.Equal("https://example.com", result.Data.Value);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void Should_Return_Failure_With_Invalid_URL(string url)
    {
        var result = ProfileUri.Create(url);
        Assert.False(result.Success);
        Assert.NotNull(result.ErrorMessage);
        Assert.Contains(PlayerError.InvalidProfileUri()._message, result.ErrorMessage);
    }
}