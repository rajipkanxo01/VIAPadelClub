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
        var fakeDateProvider = new FakeDateProvider(DateOnly.FromDateTime(DateTime.Today));
        var scheduleResult = DailySchedule.CreateSchedule(fakeDateProvider);
        var schedule = scheduleResult.Data;

        var courtNameResult = CourtName.Create("S1");
        schedule.listOfCourts.Add(Court.Create(courtNameResult.Data));

        // Act
        schedule.Activate(fakeDateProvider);

        // Assert
        Assert.Equal(ScheduleStatus.Active, schedule.status);
    }

    [Fact]
    public void Activate_ShouldFail_WhenScheduleIsInPast()
    {
        // Arrange
        var fakeDateProvider = new FakeDateProvider(DateOnly.FromDateTime(DateTime.Today));
        var schedule = DailySchedule.CreateSchedule(fakeDateProvider).Data;
        schedule.scheduleDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-1));

        // Act
        var result = schedule.Activate(fakeDateProvider);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(ErrorMessage.PastScheduleCannotBeActivated()._message, result.ErrorMessage);
    }
    
    [Fact]
    public void Activate_ShouldFail_WhenNoCourtsAvailable()
    {
        // Arrange
        var fakeDateProvider = new FakeDateProvider(DateOnly.FromDateTime(DateTime.Today));
        var schedule = DailySchedule.CreateSchedule(fakeDateProvider).Data;

        // Act
        var result = schedule.Activate(fakeDateProvider);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(ErrorMessage.NoCourtAvailable()._message, result.ErrorMessage);
    }
    

    [Fact]
    public void Activate_ShouldFail_WhenScheduleAlreadyActive()
    {
        // Arrange
        var fakeDateProvider = new FakeDateProvider(DateOnly.FromDateTime(DateTime.Today));
        var schedule = DailySchedule.CreateSchedule(fakeDateProvider).Data;
        var courtNameResult = CourtName.Create("S1");
        schedule.listOfCourts.Add(Court.Create(courtNameResult.Data));
        schedule.status = ScheduleStatus.Active;

        // Act
        var result = schedule.Activate(fakeDateProvider);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(ErrorMessage.ScheduleAlreadyActive()._message, result.ErrorMessage);
    }
    
    [Fact]
    public void Activate_ShouldFail_WhenScheduleIsDeleted()
    {
        // Arrange
        var fakeDateProvider = new FakeDateProvider(DateOnly.FromDateTime(DateTime.Today));
        var schedule = DailySchedule.CreateSchedule(fakeDateProvider).Data;
        var courtNameResult = CourtName.Create("S1");
        schedule.listOfCourts.Add(Court.Create(courtNameResult.Data));
        schedule.isDeleted = true;

        // Act
        var result = schedule.Activate(fakeDateProvider);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(ErrorMessage.ScheduleIsDeleted()._message, result.ErrorMessage);
    }
}