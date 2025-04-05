using UnitTests.Features.Helpers.Factory;
using UnitTests.Features.Helpers.Repository;
using VIAPadelClub.Core.Application.CommandDispatching.Commands.DailySchedule;
using VIAPadelClub.Core.Application.Features.Daily_Schedule;
using VIAPadelClub.Core.Domain.Aggregates.DailySchedules.Values;
using VIAPadelClub.Core.Tools.OperationResult;
using Xunit;

namespace UnitTests.Features.DailyScheduleTest.SetPartOfDailyScheduleAsVIPOnly;

public class AddVipTimeSlotHandlerTest
{
    [Fact]
    public void ShouldSucceed_WhenValidCommandIsProvided()
    {
        // Arrange
        var dailySchedule = DailyScheduleBuilder.CreateValid().Activate().BuildAsync().Data;
        var scheduleId = ScheduleId.FromGuid(dailySchedule.scheduleId.Value).Value;
        var command = AddVipTimeSlotCommand.Create(scheduleId.ToString(), "16:00", "17:00").Data;

        var scheduleRepository = new FakeDailyScheduleRepository();
        scheduleRepository.AddAsync(dailySchedule);
        
        var handler = new AddVipTimeSlotsHandler(scheduleRepository);
        
        // Act
        var result = handler.HandleAsync(command).Result;

        // Assert
        Assert.True(result.Success);
        Assert.Single(dailySchedule.vipTimeRanges);
    }
    
    [Fact]
    public void ShouldFail_WhenInvalidCommandIsProvided()
    {
        // Arrange
        var dailySchedule = DailyScheduleBuilder.CreateValid().Activate().BuildAsync().Data;
        var scheduleId = ScheduleId.FromGuid(dailySchedule.scheduleId.Value);
        var command = AddVipTimeSlotCommand.Create(scheduleId.Value.ToString(), "16:15", "17:00").Data;

        var scheduleRepository = new FakeDailyScheduleRepository();
        scheduleRepository.AddAsync(dailySchedule);
        
        var handler = new AddVipTimeSlotsHandler(scheduleRepository);
        
        // Act
        var result = handler.HandleAsync(command).Result;

        // Assert
        Assert.False(result.Success);
    }
}