using FluentAssertions;
using UnitTests.Features.Helpers;
using VIAPadelClub.Core.Domain.Aggregates.DailySchedules;
using VIAPadelClub.Core.Domain.Aggregates.DailySchedules.Entities;
using VIAPadelClub.Core.Domain.Aggregates.DailySchedules.Values;
using VIAPadelClub.Core.Tools.OperationResult;
using Xunit;

namespace UnitTests.Features.DailyScheduleTest;

public class RemoveAvailableCourtAggregateTest
{
    [Fact]
    public void Should_Remove_Court_Successfully_When_Valid() 
    {
        // Arrange
        var fakeDateProvider = new FakeDateProvider(DateOnly.FromDateTime(DateTime.Today));
        var schedule = DailySchedule.CreateSchedule(fakeDateProvider).Data;
        var fakeScheduleFinder = new FakeScheduleFinder();
        fakeScheduleFinder.AddSchedule(schedule);
        var court = Court.Create(CourtName.Create("S1").Data);
        schedule.status = ScheduleStatus.Draft;
        schedule.listOfAvailableCourts.Add(court);

        // Act
        var result = schedule.removeAvailableCourt(court, fakeDateProvider, fakeScheduleFinder);

        // Assert
        result.Success.Should().BeTrue();
        schedule.listOfAvailableCourts.Should().NotContain(court);
    }
    
    [Fact]
    public void Should_Not_Remove_Court_If_Not_Exists()
    {
        // Arrange
        var fakeDateProvider = new FakeDateProvider(DateOnly.FromDateTime(DateTime.Today));
        var schedule = DailySchedule.CreateSchedule(fakeDateProvider).Data;
        var fakeScheduleFinder = new FakeScheduleFinder();
        fakeScheduleFinder.AddSchedule(schedule);
        var court = Court.Create(CourtName.Create("S1").Data);
        schedule.status = ScheduleStatus.Draft;

        // Act
        var result = schedule.removeAvailableCourt(court, fakeDateProvider, fakeScheduleFinder);

        // Assert
        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().Be(ErrorMessage.NoCourtAvailable()._message);
    }
    
    [Fact]
    public void Should_Not_Remove_Court_If_Schedule_Is_Past()
    {
        // Arrange
        var fakeDateProvider = new FakeDateProvider(DateOnly.FromDateTime(DateTime.Today.AddDays(-1)));
        var schedule = DailySchedule.CreateSchedule(fakeDateProvider).Data;
        var fakeScheduleFinder = new FakeScheduleFinder();
        fakeScheduleFinder.AddSchedule(schedule);
        var court = Court.Create(CourtName.Create("S1").Data);
        schedule.listOfAvailableCourts.Add(court);
        schedule.status = ScheduleStatus.Draft;
        var fakeCurrentDateProvider= new FakeDateProvider(DateOnly.FromDateTime(DateTime.Today));

        // Act
        var result = schedule.removeAvailableCourt(court, fakeCurrentDateProvider, fakeScheduleFinder);

        // Assert
        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().Be(ErrorMessage.PastScheduleCannotBeUpdated()._message);
    }
    
    [Fact]
    public void Should_Remove_Court_When_Only_One_Court_Present() // S3
    {
        // Arrange
        var fakeDateProvider = new FakeDateProvider(DateOnly.FromDateTime(DateTime.Today));
        var schedule = DailySchedule.CreateSchedule(fakeDateProvider).Data;
        var fakeScheduleFinder = new FakeScheduleFinder();
        fakeScheduleFinder.AddSchedule(schedule);
        var court = Court.Create(CourtName.Create("S1").Data);
        schedule.status = ScheduleStatus.Draft;
        schedule.listOfAvailableCourts.Add(court);

        // Act
        var result = schedule.removeAvailableCourt(court, fakeDateProvider, fakeScheduleFinder);

        // Assert
        result.Success.Should().BeTrue();
        schedule.listOfAvailableCourts.Should().BeEmpty();
    }
    
    [Fact]
    public void Should_Remove_Selected_Court_And_Keep_Other_Courts_Intact() // S4
    {
        // Arrange
        var fakeDateProvider = new FakeDateProvider(DateOnly.FromDateTime(DateTime.Today));
        var schedule = DailySchedule.CreateSchedule(fakeDateProvider).Data;
        var fakeScheduleFinder = new FakeScheduleFinder();
        fakeScheduleFinder.AddSchedule(schedule);
        var court = Court.Create(CourtName.Create("S1").Data);
        var court1 = Court.Create(CourtName.Create("S2").Data);
        var court2 = Court.Create(CourtName.Create("S3").Data);
        schedule.status = ScheduleStatus.Draft;
        schedule.listOfAvailableCourts.Add(court);
        schedule.listOfAvailableCourts.Add(court1);
        schedule.listOfAvailableCourts.Add(court2);

        // Act
        var result = schedule.removeAvailableCourt(court1, fakeDateProvider, fakeScheduleFinder);

        // Assert
        result.Success.Should().BeTrue();
        schedule.listOfAvailableCourts.Should().NotContain(court1);
        schedule.listOfAvailableCourts.Should().Contain(court2);
    }
}