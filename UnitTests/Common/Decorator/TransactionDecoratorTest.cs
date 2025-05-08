using System.Windows.Input;
using Moq;
using VIAPadelClub.Core.Application.Decorator;
using VIAPadelClub.Core.Application.Dispatcher;
using VIAPadelClub.Core.Domain.Common;
using VIAPadelClub.Core.Domain.Common.Repositories;
using VIAPadelClub.Core.Tools.OperationResult;
using Xunit;

namespace UnitTests.Common.Decorator;

public class TransactionDecoratorTest
{
    private readonly Mock<ICommandDispatcher> _mockDispatcher;
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly TransactionDecorator _transactionDecorator;

    public TransactionDecoratorTest()
    {
        _mockDispatcher = new Mock<ICommandDispatcher>();
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _transactionDecorator = new TransactionDecorator(_mockDispatcher.Object, _mockUnitOfWork.Object);
    }

    [Fact]
    public async Task Should_Call_SaveChanges_When_Command_Is_Successful()
    {
        // Arrange
        var command = new Mock<ICommand>().Object;
        _mockDispatcher
            .Setup(d => d.DispatchAsync(command))
            .ReturnsAsync(Result.Ok());

        // Act
        var result = await _transactionDecorator.DispatchAsync(command);

        // Assert
        Assert.True(result.Success);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task Should_Not_Call_SaveChanges_When_Command_Fails()
    {
        // Arrange
        var command = new Mock<ICommand>().Object;
        _mockDispatcher
            .Setup(d => d.DispatchAsync(command))
            .ReturnsAsync(Result.Fail("Error"));

        // Act
        var result = await _transactionDecorator.DispatchAsync(command);

        // Assert
        Assert.False(result.Success);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task DispatchAsync_Should_SaveChanges_Only_Once_Even_If_Called_Multiple_Times()
    {
        // Arrange
        var command = new Mock<ICommand>().Object;
        _mockDispatcher
            .Setup(d => d.DispatchAsync(command))
            .ReturnsAsync(Result.Ok());

        // Act
        await _transactionDecorator.DispatchAsync(command);
        await _transactionDecorator.DispatchAsync(command);

        // Assert
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
    }
}