using UnitTests.Features.Helpers;
using UnitTests.Features.Helpers.Factory;
using UnitTests.Features.Helpers.Repository;
using VIAPadelClub.Core.Application.CommandDispatching.Commands.DailySchedule;
using VIAPadelClub.Core.Application.Features.Daily_Schedule;
using Xunit;

namespace UnitTests.Features.DailyScheduleTest.DeleteDailySchedule;

public class DeleteDailyScheduleHandlerTest
{
    [Fact]
    public async Task HandleAsync_ShouldSucceed_WhenScheduleIsValidForDeletion()
    {
        // Arrange
        var schedule = DailyScheduleBuilder.CreateValid().BuildAsync().Data;
        var repo = new FakeDailyScheduleRepository();
        await repo.AddAsync(schedule);

        var command = DeleteDailyScheduleCommand.Create(schedule.Id.ToString()).Data;
        var handler = new DeleteDailyScheduleCommandHandler(
            repo,
            new FakeUnitOfWork(),
            new FakeDateProvider(DateOnly.FromDateTime(DateTime.Today)),
            new FakeTimeProvider(TimeOnly.FromDateTime(DateTime.Now))
        );

        // Act
        var result = await handler.HandleAsync(command);

        // Assert
        Assert.True(result.Success);
    }
}