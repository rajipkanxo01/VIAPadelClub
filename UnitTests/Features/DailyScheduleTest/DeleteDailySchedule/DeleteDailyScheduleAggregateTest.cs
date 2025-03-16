using UnitTests.Features.Helpers;

namespace UnitTests.Features.DailyScheduleTest.DeleteDailySchedule;

using VIAPadelClub.Core.Domain.Aggregates.DailySchedules;
using VIAPadelClub.Core.Domain.Aggregates.DailySchedules.Values;
using Xunit;

public class DeleteDailyScheduleAggregateTest {
    
    [Fact]
    public void Should_Soft_Delete_Active_Schedule()
    {
        // Arrange
        var fakeDateProvider = new FakeDateProvider(DateOnly.FromDateTime(DateTime.Today)); 
        
        var schedule = DailySchedule.CreateSchedule(fakeDateProvider).Data;
        schedule.status = ScheduleStatus.Active;
        schedule.scheduleDate = fakeDateProvider.Today().AddDays(1);

        // Act
        var result = schedule.DeleteSchedule(fakeDateProvider);

        // Assert
        Assert.True(result.Success);
        Assert.True(schedule.isDeleted);
        Assert.Empty(schedule.listOfCourts);
    }

    [Fact]
    public void Should_Fail_To_Delete_Already_Deleted_Schedule()
    {
        // Arrange
        var fakeDateProvider = new FakeDateProvider(DateOnly.FromDateTime(DateTime.Today)); 
        
        var schedule = DailySchedule.CreateSchedule(fakeDateProvider).Data!;
        schedule.isDeleted = true;
        
        // Act
        var result = schedule.DeleteSchedule(fakeDateProvider);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("Schedule is already deleted.", result.ErrorMessage);
    }

    [Fact]
    public void Should_Soft_Delete_Draft_Schedule()
    {
        // Arrange
        var fakeDateProvider = new FakeDateProvider(DateOnly.FromDateTime(DateTime.Today)); 
        
        var schedule = DailySchedule.CreateSchedule(fakeDateProvider).Data!;
        schedule.status = ScheduleStatus.Draft;
        schedule.scheduleDate = fakeDateProvider.Today();

        // Act
        var result = schedule.DeleteSchedule(fakeDateProvider);

        // Assert
        Assert.True(result.Success);
        Assert.True(schedule.isDeleted);
        Assert.Empty(schedule.listOfCourts);
    }

    [Fact]
    public void Should_Fail_To_Delete_Past_Schedule()
    {
        // Arrange
        var fakeDateProvider = new FakeDateProvider(DateOnly.FromDateTime(DateTime.Today)); 
        
        var schedule = DailySchedule.CreateSchedule(fakeDateProvider).Data;
        schedule.scheduleDate = fakeDateProvider.Today().AddDays(-1);

        // Act
        var result = schedule.DeleteSchedule(fakeDateProvider);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("Cannot delete a schedule from the past.", result.ErrorMessage);
    }

    [Fact]
    public void Should_Fail_To_Delete_Active_Schedule_On_Same_Day()
    {
        // Arrange
        var fakeDateProvider = new FakeDateProvider(DateOnly.FromDateTime(DateTime.Today)); 
        
        var schedule = DailySchedule.CreateSchedule(fakeDateProvider).Data!;
        schedule.status = ScheduleStatus.Active;
        schedule.scheduleDate = fakeDateProvider.Today();

        // Act
        var result = schedule.DeleteSchedule(fakeDateProvider);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("Cannot delete an active schedule on the same day.", result.ErrorMessage);
    }
}