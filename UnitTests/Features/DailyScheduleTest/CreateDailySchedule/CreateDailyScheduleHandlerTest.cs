using UnitTests.Features.Helpers;
using UnitTests.Features.Helpers.Repository;
using VIAPadelClub.Core.Application.CommandDispatching.Commands.DailySchedule;
using VIAPadelClub.Core.Application.Features.Daily_Schedule;
using Xunit;

namespace UnitTests.Features.DailyScheduleTest.CreateDailySchedule;

public class CreateDailyScheduleHandlerTest
{
    [Fact]
    public async Task ShouldCreateAndPersistSchedule_WhenCommandIsValid()
    {
        // Arrange
        var scheduleRepo = new FakeDailyScheduleRepository();
        var dateProvider = new FakeDateProvider(DateOnly.FromDateTime(DateTime.Today));

        var handler = new CreateDailyScheduleHandler(scheduleRepo, dateProvider);
        var commandResult = CreateDailyScheduleCommand.Create();

        Assert.True(commandResult.Success);
        var command = commandResult.Data;

        // Act
        var result = await handler.HandleAsync(command);

        // Assert
        Assert.True(result.Success);
    }
}