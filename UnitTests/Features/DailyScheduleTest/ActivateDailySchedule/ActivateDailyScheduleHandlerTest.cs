using UnitTests.Features.Helpers;
using UnitTests.Features.Helpers.Factory;
using UnitTests.Features.Helpers.Repository;
using VIAPadelClub.Core.Application.CommandDispatching.Commands.DailySchedule;
using VIAPadelClub.Core.Application.Features.Daily_Schedule;
using VIAPadelClub.Core.Domain.Aggregates.DailySchedules.Entities;
using VIAPadelClub.Core.Domain.Aggregates.DailySchedules.Values;
using VIAPadelClub.Core.Domain.Common;
using Xunit;

namespace UnitTests.Features.DailyScheduleTest.ActivateDailySchedule;

public class ActivateDailyScheduleHandlerTest
{
    [Fact]
    public void ShouldSucceed_WhenValidCommandIsProvided()
    {
        // Arrange
        var scheduleRepository = new FakeDailyScheduleRepository();
        var fakeScheduleFinder = new FakeScheduleFinder(scheduleRepository);
        var dateProvider=new FakeDateProvider(DateOnly.FromDateTime(DateTime.Today));
        
        var court = Court.Create(CourtName.Create("D1").Data).Data;
        var dailySchedule = DailyScheduleBuilder.CreateValid().WithDateProvider(dateProvider).WithCourt(court)
            .WithScheduleFinder(fakeScheduleFinder).BuildAsync().Data;
        var scheduleId = ScheduleId.FromGuid(dailySchedule.scheduleId.Value);
        scheduleRepository.AddAsync(dailySchedule);
        
        var handler = new ActivateDailyScheduleHandler(scheduleRepository,dateProvider);
        var command = ActivateDailyScheduleCommand.Create(scheduleId.Value.ToString());
        
        // Act
        var result = handler.HandleAsync(command.Data).Result;

        // Assert
        Assert.True(result.Success);
    }
}