using VIAPadelClub.Core.Domain.Aggregates.DailySchedules;
using VIAPadelClub.Core.Domain.Aggregates.DailySchedules.Entities;
using VIAPadelClub.Core.Domain.Aggregates.DailySchedules.Values;
using VIAPadelClub.Core.Tools.OperationResult;
using Xunit;

public class DailyScheduleTests
{
    [Fact]
    public void Activate_ShouldSetStatusToActive_WhenConditionsAreMet()
    {
        // Arrange
        var scheduleResult = DailySchedule.CreateSchedule();
        var schedule = scheduleResult.Data;

        var courtNameResult = CourtName.Create("S1");
        schedule.listOfCourts.Add(Court.Create(courtNameResult.Data));

        // Act
        schedule.Activate();

        // Assert
        Assert.Equal(ScheduleStatus.Active, schedule.status);
    }

    [Fact]
    public void Activate_ShouldFail_WhenScheduleIsInPast()
    {
        // Arrange
        var schedule = DailySchedule.CreateSchedule().Data;
        schedule.scheduleDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-1));

        // Act
        var result = schedule.Activate();

        // Assert
        Assert.False(result.Success);
        Assert.Equal(ErrorMessage.PastScheduleCannotBeActivated()._message, result.ErrorMessage);
    }
    
    [Fact]
    public void Activate_ShouldFail_WhenNoCourtsAvailable()
    {
        // Arrange
        var schedule = DailySchedule.CreateSchedule().Data;

        // Act
        var result = schedule.Activate();

        // Assert
        Assert.False(result.Success);
        Assert.Equal(ErrorMessage.NoCourtAvailable()._message, result.ErrorMessage);
    }
    

    [Fact]
    public void Activate_ShouldFail_WhenScheduleAlreadyActive()
    {
        // Arrange
        var schedule = DailySchedule.CreateSchedule().Data;
        var courtNameResult = CourtName.Create("S1");
        schedule.listOfCourts.Add(Court.Create(courtNameResult.Data));
        schedule.status = ScheduleStatus.Active;

        // Act
        var result = schedule.Activate();

        // Assert
        Assert.False(result.Success);
        Assert.Equal(ErrorMessage.ScheduleAlreadyActive()._message, result.ErrorMessage);
    }
    
    [Fact]
    public void Activate_ShouldFail_WhenScheduleIsDeleted()
    {
        // Arrange
        var schedule = DailySchedule.CreateSchedule().Data;
        var courtNameResult = CourtName.Create("S1");
        schedule.listOfCourts.Add(Court.Create(courtNameResult.Data));
        schedule.isDeleted = true;

        // Act
        var result = schedule.Activate();

        // Assert
        Assert.False(result.Success);
        Assert.Equal(ErrorMessage.ScheduleIsDeleted()._message, result.ErrorMessage);
    }
}
