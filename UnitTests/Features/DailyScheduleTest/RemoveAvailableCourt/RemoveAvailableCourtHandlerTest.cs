using UnitTests.Features.Helpers;
using UnitTests.Features.Helpers.Factory;
using UnitTests.Features.Helpers.Repository;
using VIAPadelClub.Core.Application.CommandDispatching.Commands.DailySchedule;
using VIAPadelClub.Core.Application.Features.Daily_Schedule;
using VIAPadelClub.Core.Domain.Aggregates.DailySchedules.Entities;
using VIAPadelClub.Core.Domain.Aggregates.DailySchedules.Values;
using Xunit;

namespace UnitTests.Features.DailyScheduleTest.RemoveAvailableCourt;

public class RemoveAvailableCourtHandlerTest
{
    [Fact]
    public void ShouldSucceed_WhenValidCommandIsProvided()
    {
        // Arrange
        var scheduleRepository = new FakeDailyScheduleRepository();
        var fakeScheduleFinder = new FakeScheduleFinder(scheduleRepository);
        var dateProvider = new FakeDateProvider(DateOnly.FromDateTime(DateTime.Today));
        
        var court = Court.Create(CourtName.Create("D1").Data).Data;
        var dailySchedule = DailyScheduleBuilder.CreateValid().WithDateProvider(dateProvider).Activate().WithCourt(court)
            .WithScheduleFinder(fakeScheduleFinder).BuildAsync().Data;
        
        var timeOfRemoval = new TimeOnly(9, 0, 0);
        scheduleRepository.AddAsync(dailySchedule);
        
        var command = RemoveAvailableCourtCommand.Create(dailySchedule.scheduleId.ToString(),court.Name.Value,timeOfRemoval.ToString()).Data;
        var handler = new RemoveAvailableCourtHandler(scheduleRepository,dateProvider);
        
        // Act
        var result = handler.HandleAsync(command).Result;

        // Assert
        Assert.True(result.Success);
    }
}