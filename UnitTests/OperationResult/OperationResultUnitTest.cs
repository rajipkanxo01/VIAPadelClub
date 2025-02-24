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
        var result = Result.Success();

        // Assert
        result._success.Should().BeTrue();
        result._error.Should().BeNull();
    }

    [Fact]
    public void Result_Fail_ReturnsFailureResult()
    {
        // Arrange
        var error = new Error(1, "Something went wrong!");

        // Act
        var result = Result.Fail(error);

        // Assert
        Assert.False(result._success);
        Assert.Equal(error, result._error);
    }
    
    [Fact]
    public void SuccessT_ShouldCreateSuccessfulResultWithData()
    {
        // Arrange
        var expectedData = 42;

        // Act
        var result = Result<int>.Success(expectedData);

        // Assert
        Assert.True(result._success);
        Assert.Null(result._error);
        Assert.Equal(expectedData, result._data);
    }

    [Fact]
    public void FailT_ShouldCreateFailedResultWithError()
    {
        // Arrange
        var error = new Error(2, "Failed to retrieve data");

        // Act
        var result = Result<int>.Fail(error);

        // Assert
        Assert.False(result._success);
        Assert.Equal(error, result._error);
        result._data.Should().Be(0);
    }

}