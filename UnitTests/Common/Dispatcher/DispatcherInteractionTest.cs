using Microsoft.Extensions.DependencyInjection;
using Moq;
using VIAPadelClub.Core.Application.CommandDispatching;
using VIAPadelClub.Core.Application.CommandDispatching.Commands.DailySchedule;
using VIAPadelClub.Core.Application.CommandDispatching.Commands.Player;
using VIAPadelClub.Core.Application.Dispatcher;
using VIAPadelClub.Core.Domain.Aggregates.DailySchedules;
using VIAPadelClub.Core.Domain.Common;
using VIAPadelClub.Core.Domain.Common.Repositories;
using VIAPadelClub.Core.Tools.OperationResult;
using Xunit;

namespace UnitTests.Common.Dispatcher;

public class DispatcherInteractionTest
{
    [Fact]
    public async Task Should_Call_Correct_Handler_With_Successful_Result() 
    {
        // Arrange
        var command = CreateDailyScheduleCommand.Create().Data;
        var mock = new Mock<ICommandHandler<CreateDailyScheduleCommand>>();
        mock.Setup(h => h.HandleAsync(It.IsAny<CreateDailyScheduleCommand>())).ReturnsAsync(Result.Ok);
        
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockDailyScheduleRepository = new Mock<IDailyScheduleRepository>();

        var serviceProvider = new ServiceCollection()
            .AddSingleton(_ => mock.Object)
            .AddSingleton(_ => mockUnitOfWork.Object)
            .AddSingleton(_ => mockDailyScheduleRepository.Object)
            .BuildServiceProvider();
        
        var commandDispatcher = new CommandDispatcher(serviceProvider);
        
        //Act
        var result = await commandDispatcher.DispatchAsync(command);
        
        // Assert
        Assert.True(result.Success);
        mock.Verify(h => h.HandleAsync(It.IsAny<CreateDailyScheduleCommand>()), Times.Once);
    }
    
    
    [Fact]
    public async Task Should_Throw_Exception_When_Wrong_Handler_Is_Registered() 
    {
        //Arrange
        var command = LiftsBlacklistsPlayerCommand.Create("via@via.dk").Data;
        var mock = new Mock<ICommandHandler<CreateDailyScheduleCommand>>();
        
        var serviceProvider = new ServiceCollection()
            .AddSingleton(_ => mock.Object)
            .BuildServiceProvider();
        
        var commandDispatcher = new CommandDispatcher(serviceProvider);
        
        // Act
        async Task Act() {
            await commandDispatcher.DispatchAsync(command);
        }
        // Assert
        await Assert.ThrowsAsync<InvalidOperationException>(Act);
    }

    [Fact]
    public async Task Should_Throw_Exception_When_Handler_Is_Not_Registered()
    {
        // Arrange
        var serviceCollection = new ServiceCollection();
        var serviceProvider = serviceCollection.BuildServiceProvider();
        
        ICommandDispatcher commandDispatcher = new CommandDispatcher(serviceProvider);
        CreateDailyScheduleCommand command = CreateDailyScheduleCommand.Create().Data;
        
        // Act
        async Task Act() {
            await commandDispatcher.DispatchAsync(command);
        }
        
        // Assert
        await Assert.ThrowsAsync<InvalidOperationException>(Act);
    }

    [Fact]
    public async Task Should_Succeed_When_Multiple_Handlers_Are_Registered_With_Multiple_Commands()
    {
        // Arrange
        var command1 = CreateDailyScheduleCommand.Create().Data;
        var command2 = LiftsBlacklistsPlayerCommand.Create("via@via.dk").Data;
        
        var mockHandler1 = new Mock<ICommandHandler<CreateDailyScheduleCommand>>();
        var mockHandler2 = new Mock<ICommandHandler<LiftsBlacklistsPlayerCommand>>();
        
        mockHandler1.Setup(h => h.HandleAsync(It.IsAny<CreateDailyScheduleCommand>())).ReturnsAsync(Result.Ok);
        mockHandler2.Setup(h => h.HandleAsync(It.IsAny<LiftsBlacklistsPlayerCommand>())).ReturnsAsync(Result.Ok);
        
        var serviceProvider = new ServiceCollection()
            .AddSingleton(_ => mockHandler1.Object)
            .AddSingleton(_ => mockHandler2.Object)
            .BuildServiceProvider();
        
        var commandDispatcher = new CommandDispatcher(serviceProvider);
        
        // Act
        var result1 = await commandDispatcher.DispatchAsync(command1);
        var result2 = await commandDispatcher.DispatchAsync(command2);
        
        // Assert
        Assert.True(result1.Success);
        Assert.True(result2.Success);
        mockHandler1.Verify(h => h.HandleAsync(It.IsAny<CreateDailyScheduleCommand>()), Times.Once);
        mockHandler2.Verify(h => h.HandleAsync(It.IsAny<LiftsBlacklistsPlayerCommand>()), Times.Once);
    }
    
    [Fact]
    public async Task Should_Call_Only_One_Handler_When_Multiple_Handlers_Are_Registered()
    {
        // Arrange
        var command = CreateDailyScheduleCommand.Create().Data;
        var mockHandler1 = new Mock<ICommandHandler<CreateDailyScheduleCommand>>();
        var mockHandler2 = new Mock<ICommandHandler<LiftsBlacklistsPlayerCommand>>();
        
        mockHandler1.Setup(h => h.HandleAsync(It.IsAny<CreateDailyScheduleCommand>())).ReturnsAsync(Result.Ok);
        mockHandler2.Setup(h => h.HandleAsync(It.IsAny<LiftsBlacklistsPlayerCommand>())).ReturnsAsync(Result.Ok);
        
        var serviceProvider = new ServiceCollection()
            .AddSingleton(_ => mockHandler1.Object)
            .AddSingleton(_ => mockHandler2.Object)
            .BuildServiceProvider();
        
        var commandDispatcher = new CommandDispatcher(serviceProvider);
        
        // Act
        var result = await commandDispatcher.DispatchAsync(command);
        
        // Assert
        Assert.True(result.Success);
        mockHandler1.Verify(h => h.HandleAsync(It.IsAny<CreateDailyScheduleCommand>()), Times.Once);
        mockHandler2.Verify(h => h.HandleAsync(It.IsAny<LiftsBlacklistsPlayerCommand>()), Times.Never);
    }
}