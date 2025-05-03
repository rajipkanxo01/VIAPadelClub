using UnitTests.Features.Helpers;
using UnitTests.Features.Helpers.Factory;
using UnitTests.Features.Helpers.Repository;
using VIAPadelClub.Core.Application.CommandDispatching.Commands.DailySchedule;
using VIAPadelClub.Core.Application.Features.Daily_Schedule;
using VIAPadelClub.Core.Domain.Aggregates.DailySchedules.Values;
using Xunit;

namespace UnitTests.Features.DailyScheduleTest.DeleteDailySchedule;

public class DeleteDailyScheduleHandlerTest
{
    [Fact]
    public async Task HandleAsync_ShouldSucceed_WhenScheduleIsValidForDeletion()
    {
        // Arrange
        var schedule = DailyScheduleBuilder.CreateValid().BuildAsync().Data;
        var scheduleId = ScheduleId.FromGuid(schedule.ScheduleId.Value);
        var repo = new FakeDailyScheduleRepository();
        await repo.AddAsync(schedule);

        var command = DeleteDailyScheduleCommand.Create(scheduleId.Value.ToString()).Data;
        
        var handler = new DeleteDailyScheduleCommandHandler(
            repo,
            new FakeDateProvider(DateOnly.FromDateTime(DateTime.Today)),
            new FakeTimeProvider(TimeOnly.FromDateTime(DateTime.Now))
        );

        // Act
        var result = await handler.HandleAsync(command);

        // Assert
        Assert.True(result.Success);
    }
}