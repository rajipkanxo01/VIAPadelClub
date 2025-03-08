using FluentAssertions;
using VIAPadelClub.Core.Tools.OperationResult;
using Xunit;

namespace UnitTests.OperationResult;

public class OperationResultUnitTest
{
    [Fact]
    public void Success_ShouldReturnSuccessfulResult()
    {
        // Act
        var result = Result.Ok();

        // Assert
        result.Success.Should().Be(true);
    }

    [Fact]
    public void Result_Fail_ReturnsFailureResult()
    {
        // Arrange
        var errorMessage = "Something went wrong!";

        // Act
        var result = Result.Fail(errorMessage);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(errorMessage, result.ErrorMessage);
    }
    
    [Fact]
    public void SuccessT_ShouldCreateSuccessfulResultWithData()
    {
        // Arrange
        var expectedData = 42;

        // Act
        var result = Result<int>.Ok(expectedData);

        // Assert
        Assert.True(result.Success);
        Assert.Empty(result.ErrorMessage);
        Assert.Equal(expectedData, result.Data);
    }

    [Fact]
    public void FailT_ShouldCreateFailedResultWithError()
    {
        // Arrange
        var errorMessage = "Failed to retrieve data!";

        // Act
        var result = Result<int>.Fail(errorMessage);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(errorMessage, result.ErrorMessage);
    }

}