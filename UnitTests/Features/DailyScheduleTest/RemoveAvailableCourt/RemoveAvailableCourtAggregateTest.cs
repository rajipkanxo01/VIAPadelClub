﻿using FluentAssertions;
using UnitTests.Features.Helpers;
using UnitTests.Features.Helpers.Factory;
using UnitTests.Features.Helpers.Repository;
using VIAPadelClub.Core.Domain.Aggregates.DailySchedules;
using VIAPadelClub.Core.Domain.Aggregates.DailySchedules.Entities;
using VIAPadelClub.Core.Domain.Aggregates.DailySchedules.Values;
using VIAPadelClub.Core.Domain.Aggregates.Players;
using VIAPadelClub.Core.Domain.Aggregates.Players.Values;
using VIAPadelClub.Core.Tools.OperationResult;
using Xunit;

namespace UnitTests.Features.DailyScheduleTest.RemoveAvailableCourt;

public class RemoveAvailableCourtAggregateTest
{
    private readonly FakeDailyScheduleRepository dailyScheduleRepository= new FakeDailyScheduleRepository();
    private readonly FakePlayerRepository playerRepository = new FakePlayerRepository();
    [Fact]
    public async Task Should_Remove_Court_Successfully_When_Valid() 
    {
        // Arrange
        var scheduleId = ScheduleId.FromGuid(Guid.NewGuid());
        
        var fakeDateProvider = new FakeDateProvider(DateOnly.FromDateTime(DateTime.Today));
        var fakeScheduleFinder = new FakeScheduleFinder(dailyScheduleRepository);

        var schedule = DailySchedule.CreateSchedule(fakeDateProvider, scheduleId).Data;
        fakeScheduleFinder.AddSchedule(schedule);
        
        var court = Court.Create(CourtName.Create("S1").Data).Data;
        schedule.listOfCourts.Add(court);
        
        schedule.Activate(fakeDateProvider);
        
        // Act
        var result = schedule.RemoveAvailableCourt(court, fakeDateProvider,new TimeOnly(9,0));

        // Assert
        result.Success.Should().BeTrue();
        schedule.listOfAvailableCourts.Should().NotContain(court);
    }
    
    [Fact]
    public void Should_Not_Remove_Court_If_Not_Exists()
    {
        // Arrange
        var fakeDateProvider = new FakeDateProvider(DateOnly.FromDateTime(DateTime.Today));
        var fakeScheduleFinder = new FakeScheduleFinder(dailyScheduleRepository);
        var scheduleId = ScheduleId.FromGuid(Guid.NewGuid());
        
        var schedule = DailySchedule.CreateSchedule(fakeDateProvider, scheduleId).Data;
        fakeScheduleFinder.AddSchedule(schedule);
        
        var court = Court.Create(CourtName.Create("S1").Data).Data;
        schedule.status = ScheduleStatus.Draft;

        // Act
        var result = schedule.RemoveAvailableCourt(court, fakeDateProvider,new TimeOnly(15,0));

        // Assert
        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().Be(DailyScheduleError.NoCourtAvailable()._message);
    }
    
    [Fact]
    public void Should_Not_Remove_Court_If_Schedule_Is_Past()
    {
        // Arrange
        var fakeDateProvider = new FakeDateProvider(DateOnly.FromDateTime(DateTime.Today.AddDays(-1)));
        var scheduleId = ScheduleId.FromGuid(Guid.NewGuid());
        var schedule = DailySchedule.CreateSchedule(fakeDateProvider, scheduleId).Data;
        var fakeScheduleFinder = new FakeScheduleFinder(dailyScheduleRepository);
        fakeScheduleFinder.AddSchedule(schedule);
        
        var court = Court.Create(CourtName.Create("S1").Data).Data;
        
        schedule.listOfCourts.Add(court);
        schedule.status = ScheduleStatus.Draft;
        var fakeCurrentDateProvider= new FakeDateProvider(DateOnly.FromDateTime(DateTime.Today));

        // Act
        var result = schedule.RemoveAvailableCourt(court, fakeCurrentDateProvider,new TimeOnly(15,0));

        // Assert
        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().Be(DailyScheduleError.PastScheduleCannotBeUpdated()._message);
    }
    
    [Fact]
    public async Task Should_Remove_Court_When_Only_One_Court_Present() // S3
    {
        // Arrange
        var fakeDateProvider = new FakeDateProvider(DateOnly.FromDateTime(DateTime.Today));
        var fakeScheduleFinder = new FakeScheduleFinder(dailyScheduleRepository);
        var playerFinder = new FakePlayerFinder(playerRepository);
        var scheduleId = ScheduleId.FromGuid(Guid.NewGuid());

        var schedule = DailySchedule.CreateSchedule(fakeDateProvider, scheduleId).Data;

        var player = (await PlayerBuilder.CreateValid().BuildAsync()).Data;
        playerFinder.AddPlayer(player);

        fakeScheduleFinder.AddSchedule(schedule);
        
        schedule.availableFrom = new TimeOnly(9,0);
        schedule.availableUntil = new TimeOnly(17,0);
        
        var court = Court.Create(CourtName.Create("S1").Data).Data;
        schedule.listOfCourts.Add(court);
        
        schedule.Activate(fakeDateProvider);
        
        // Act
        var result = schedule.RemoveAvailableCourt(court, fakeDateProvider,new TimeOnly(9,0));

        // Assert
        result.Success.Should().BeTrue();
        schedule.listOfAvailableCourts.Should().BeEmpty();
    }

    [Fact]
    public async Task Should_Remove_Selected_Court_And_Keep_Other_Courts_Intact() // S4
    {
        // Arrange
        var fakeDateProvider = new FakeDateProvider(DateOnly.FromDateTime(DateTime.Today));
        var fakeScheduleFinder = new FakeScheduleFinder(dailyScheduleRepository);
        var playerFinder = new FakePlayerFinder(playerRepository);
        
        var scheduleId = ScheduleId.FromGuid(Guid.NewGuid());

        var schedule = DailySchedule.CreateSchedule(fakeDateProvider, scheduleId).Data;
        fakeScheduleFinder.AddSchedule(schedule);

        var player = (await PlayerBuilder.CreateValid().BuildAsync()).Data;
        playerFinder.AddPlayer(player);

        var court = Court.Create(CourtName.Create("S1").Data).Data;
        var court1 = Court.Create(CourtName.Create("S2").Data).Data;
        var court2 = Court.Create(CourtName.Create("S3").Data).Data;
        
        schedule.listOfCourts.Add(court);
        schedule.listOfCourts.Add(court1);
        schedule.listOfCourts.Add(court2);
        
        schedule.Activate(fakeDateProvider);

        // Act
        var result = schedule.RemoveAvailableCourt(court1, fakeDateProvider,new TimeOnly(15,0));

        // Assert
        result.Success.Should().BeTrue();
        schedule.listOfAvailableCourts.Should().NotContain(court1);
        schedule.listOfAvailableCourts.Should().Contain(court2);
    }
    
}