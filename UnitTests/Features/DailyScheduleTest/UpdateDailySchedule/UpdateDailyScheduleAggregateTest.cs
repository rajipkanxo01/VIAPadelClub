using VIAPadelClub.Core.Tools.OperationResult;

namespace UnitTests.Features.DailyScheduleTest.UpdateDailySchedule;

using Helpers;
using VIAPadelClub.Core.Domain.Aggregates.DailySchedules;
using VIAPadelClub.Core.Domain.Aggregates.DailySchedules.Values;
using Xunit;

public class UpdateDailyScheduleAggregateTest {
    
    [Fact]
    public void Should_Update_Schedule_Date_When_Valid()
    {
        // Arrange
        var scheduleId = ScheduleId.FromGuid(Guid.NewGuid());

        var today = new DateOnly(2025, 08, 10);
        var futureDate = new DateOnly(2025, 08, 11);
        
        var fakeDateProvider = new FakeDateProvider(today);
        
        var schedule = DailySchedule.CreateSchedule(fakeDateProvider, scheduleId).Data!;
        schedule.status = ScheduleStatus.Draft;

        // Act
        var result = schedule.UpdateScheduleDateAndTime(futureDate, new TimeOnly(10, 0), new TimeOnly(14, 0), fakeDateProvider);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(futureDate, schedule.scheduleDate);
        Assert.Equal(new TimeOnly(10, 0), schedule.availableFrom);
        Assert.Equal(new TimeOnly(14, 0), schedule.availableUntil);
    }
    
    [Fact]
    public void Should_Update_Time_When_Valid()
    {
        // Arrange
        var scheduleId = ScheduleId.FromGuid(Guid.NewGuid());

        var today = new DateOnly(2025, 08, 10);
        var futureDate = new DateOnly(2025, 08, 11);
        
        var fakeDateProvider = new FakeDateProvider(today);
        
        var schedule = DailySchedule.CreateSchedule(fakeDateProvider, scheduleId).Data!;
        schedule.status = ScheduleStatus.Draft;

        // Act
        var result = schedule.UpdateScheduleDateAndTime(futureDate, new TimeOnly(08, 00), new TimeOnly(16, 00), fakeDateProvider);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(new TimeOnly(08, 00), schedule.availableFrom);
        Assert.Equal(new TimeOnly(16, 00), schedule.availableUntil);
    }
    
    [Fact]
    public void Should_Fail_When_Date_Is_In_The_Past()
    {
        // Arrange
        var scheduleId = ScheduleId.FromGuid(Guid.NewGuid());

        var today = new DateOnly(2025, 08, 10);
        var pastDate = new DateOnly(2025, 08, 09);
        
        var fakeDateProvider = new FakeDateProvider(today);
        
        var schedule = DailySchedule.CreateSchedule(fakeDateProvider, scheduleId).Data!;

        // Act
        var result = schedule.UpdateScheduleDateAndTime(pastDate, new TimeOnly(10, 0), new TimeOnly(14, 0), fakeDateProvider);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("Cannot update schedule with past date.", result.ErrorMessage);
    }
    
    [Fact]
    public void Should_Fail_When_End_Time_Is_Before_Start_Time()
    {
        // Arrange
        var scheduleId = ScheduleId.FromGuid(Guid.NewGuid());

        var today = new DateOnly(2025, 08, 10);
        var futureDate = new DateOnly(2025, 08, 11);
        
        var fakeDateProvider = new FakeDateProvider(today);
        
        var schedule = DailySchedule.CreateSchedule(fakeDateProvider, scheduleId).Data!;

        // Act
        var result = schedule.UpdateScheduleDateAndTime(futureDate, new TimeOnly(14, 0), new TimeOnly(10, 0), fakeDateProvider);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("The end time must be after the start time.", result.ErrorMessage);
    }
    
    [Fact]
    public void Should_Fail_When_Time_Interval_Is_Less_Than_60_Minutes()
    {
        // Arrange
        var scheduleId = ScheduleId.FromGuid(Guid.NewGuid());

        var today = new DateOnly(2025, 08, 10);
        var futureDate = new DateOnly(2025, 08, 11);
        
        var fakeDateProvider = new FakeDateProvider(today);
        
        var schedule = DailySchedule.CreateSchedule(fakeDateProvider, scheduleId).Data!;

        // Act
        var result = schedule.UpdateScheduleDateAndTime(futureDate, new TimeOnly(10, 0), new TimeOnly(10, 30), fakeDateProvider);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("The time interval must span 60 minutes or more.", result.ErrorMessage);
    }
    
    [Fact]
    public void Should_Fail_When_Schedule_Is_Active()
    {
        // Arrange
        var scheduleId = ScheduleId.FromGuid(Guid.NewGuid());

        var today = new DateOnly(2025, 08, 10);
        var futureDate = new DateOnly(2025, 08, 11);
        
        var fakeDateProvider = new FakeDateProvider(today);
        
        var schedule = DailySchedule.CreateSchedule(fakeDateProvider, scheduleId).Data!;
        schedule.status = ScheduleStatus.Active;

        // Act
        var result = schedule.UpdateScheduleDateAndTime(futureDate, new TimeOnly(10, 0), new TimeOnly(14, 0), fakeDateProvider);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("An active daily schedule cannot be modified, only deleted.", result.ErrorMessage);
    }
    
    [Fact]
    public void Should_Fail_When_Minutes_Are_Not_00_Or_30()
    {
        // Arrange
        var scheduleId = ScheduleId.FromGuid(Guid.NewGuid());

        var today = new DateOnly(2025, 08, 10);
        var futureDate = new DateOnly(2025, 08, 11);
        
        var fakeDateProvider = new FakeDateProvider(today);
        
        var schedule = DailySchedule.CreateSchedule(fakeDateProvider, scheduleId).Data!;

        // Act
        var result = schedule.UpdateScheduleDateAndTime(futureDate, new TimeOnly(10, 02), new TimeOnly(14, 06), fakeDateProvider);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(DailyScheduleError.ScheduleInvalidTimeSpan()._message, result.ErrorMessage);
    }
}