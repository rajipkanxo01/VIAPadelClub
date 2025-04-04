using UnitTests.Features.Helpers;
using VIAPadelClub.Core.Domain.Aggregates.DailySchedules;
using VIAPadelClub.Core.Domain.Aggregates.DailySchedules.Entities;
using VIAPadelClub.Core.Domain.Aggregates.DailySchedules.Values;
using VIAPadelClub.Core.Tools.OperationResult;
using Xunit;

namespace UnitTests.Features.DailyScheduleTest.ActivateDailySchedule;

public class DailyScheduleTests
{
    [Fact]
    public void Activate_ShouldSetStatusToActive_WhenConditionsAreMet()
    {
        // Arrange
        var scheduleId = ScheduleId.FromGuid(Guid.NewGuid());

        var fakeDateProvider = new FakeDateProvider(DateOnly.FromDateTime(DateTime.Today));
        var scheduleResult = DailySchedule.CreateSchedule(fakeDateProvider, scheduleId);
        var schedule = scheduleResult.Data;

        var courtNameResult = CourtName.Create("S1");
        schedule.listOfCourts.Add(Court.Create(courtNameResult.Data).Data);

        // Act
        schedule.Activate(fakeDateProvider);

        // Assert
        Assert.Equal(ScheduleStatus.Active, schedule.status);
    }

    [Fact]
    public void Activate_ShouldFail_WhenScheduleIsInPast()
    {
        // Arrange
        var scheduleId = ScheduleId.FromGuid(Guid.NewGuid());

        
        var fakeDateProvider = new FakeDateProvider(DateOnly.FromDateTime(DateTime.Today));
        var schedule = DailySchedule.CreateSchedule(fakeDateProvider, scheduleId).Data;
        schedule.scheduleDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-1));

        // Act
        var result = schedule.Activate(fakeDateProvider);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(DailyScheduleError.PastScheduleCannotBeActivated()._message, result.ErrorMessage);
    }
    
    [Fact]
    public void Activate_ShouldFail_WhenNoCourtsAvailable()
    {
        // Arrange
        var scheduleId = ScheduleId.FromGuid(Guid.NewGuid());
        
        var fakeDateProvider = new FakeDateProvider(DateOnly.FromDateTime(DateTime.Today));
        var schedule = DailySchedule.CreateSchedule(fakeDateProvider, scheduleId).Data;

        // Act
        var result = schedule.Activate(fakeDateProvider);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(DailyScheduleError.NoCourtAvailable()._message, result.ErrorMessage);
    }
    

    [Fact]
    public void Activate_ShouldFail_WhenScheduleAlreadyActive()
    {
        // Arrange
        var scheduleId = ScheduleId.FromGuid(Guid.NewGuid());
        
        var fakeDateProvider = new FakeDateProvider(DateOnly.FromDateTime(DateTime.Today));
        var schedule = DailySchedule.CreateSchedule(fakeDateProvider, scheduleId).Data;
        var courtNameResult = CourtName.Create("S1");
        schedule.listOfCourts.Add(Court.Create(courtNameResult.Data).Data);
        schedule.status = ScheduleStatus.Active;

        // Act
        var result = schedule.Activate(fakeDateProvider);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(DailyScheduleError.ScheduleAlreadyActive()._message, result.ErrorMessage);
    }
    
    [Fact]
    public void Activate_ShouldFail_WhenScheduleIsDeleted()
    {
        // Arrange
        var scheduleId = ScheduleId.FromGuid(Guid.NewGuid());

        var fakeDateProvider = new FakeDateProvider(DateOnly.FromDateTime(DateTime.Today));
        var schedule = DailySchedule.CreateSchedule(fakeDateProvider,scheduleId).Data;
        var courtNameResult = CourtName.Create("S1");
        schedule.listOfCourts.Add(Court.Create(courtNameResult.Data).Data);
        schedule.isDeleted = true;

        // Act
        var result = schedule.Activate(fakeDateProvider);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(DailyScheduleError.ScheduleIsDeleted()._message, result.ErrorMessage);
    }
}