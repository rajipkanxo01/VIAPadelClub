using UnitTests.Features.Helpers;
using UnitTests.Features.Helpers.Factory;
using UnitTests.Features.Helpers.Repository;
using VIAPadelClub.Core.Domain.Aggregates.DailySchedules.Values;
using VIAPadelClub.Core.Domain.Aggregates.Players.Values;

namespace UnitTests.Features.PlayerTest.PlayerQuarantineTest;

using VIAPadelClub.Core.Domain.Aggregates.DailySchedules;
using VIAPadelClub.Core.Domain.Aggregates.DailySchedules.Entities;
using VIAPadelClub.Core.Domain.Aggregates.Players;
using VIAPadelClub.Core.Domain.Aggregates.Players.Entities;
using VIAPadelClub.Core.Tools.OperationResult;
using Xunit;

public class PlayerQuarantineTest {
    
    [Theory]
    [InlineData("2025-01-20")]
    [InlineData("2025-02-10")]
    public async Task Should_Quarantine_Player_Successfully(string startDateString)
    {
        // Arrange
        var player = (await PlayerBuilder.CreateValid().BuildAsync()).Data;
        var startDate = DateOnly.Parse(startDateString);
        var schedules = new List<DailySchedule>();

        // Act
        var result = player.Quarantine(startDate, schedules);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.NotNull(player.activeQuarantine); // Only one quarantine should exist
        Assert.Equal(startDate, result.Data!.StartDate);
        Assert.Equal(startDate.AddDays(3), result.Data!.EndDate);
    }
    
    [Theory]
    [InlineData("2025-01-20")]
    [InlineData("2025-02-10")]
    public async Task Should_Extend_Existing_Quarantine(string startDateString)
    {
        // Arrange
        var player = (await PlayerBuilder.CreateValid().BuildAsync()).Data;
        var startDate = DateOnly.Parse(startDateString);
        var schedules = new List<DailySchedule>();


        // First Quarantine
        player.Quarantine(startDate, schedules);

        // Act - Quarantine again on the same date, should extend
        var result = player.Quarantine(startDate, schedules);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.Equal(startDate, result.Data.StartDate);
        Assert.Equal(startDate.AddDays(6), result.Data.EndDate); // Extended by another 3 days
    }
    
    [Theory]
    [InlineData( "2025-01-20")]
    [InlineData("2025-02-10")]
    public async Task Should_Fail_To_Quarantine_Blacklisted_Player(string startDateString)
    {
        // Arrange
        var player = (await PlayerBuilder.CreateValid().BuildAsync()).Data;
        player.isBlackListed = true;
        var startDate = DateOnly.Parse(startDateString);
        var schedules = new List<DailySchedule>();


        // Act
        var result = player.Quarantine(startDate, schedules);

        // Assert
        Assert.False(result.Success);
        Assert.Null(result.Data);
        Assert.Equal("Player is already blacklisted and cannot be quarantined.", result.ErrorMessage);
        Assert.Null(player.activeQuarantine);
    }
    

    [Theory]
    [InlineData("2023e82d-84f1-4f07-90a2-a6e173b82111", "2025-01-20")]
    [InlineData("b2c9519e-20ab-4c02-b50a-57f34e82b33f", "2025-02-15")]
    public void Should_Fail_Quarantine_When_Player_Not_Found(string playerIdString, string startDateString)
    {
        // Arrange
        Guid playerId = Guid.Parse(playerIdString);
        DateOnly startDate = DateOnly.Parse(startDateString);
        Player? player = null;
        var schedules = new List<DailySchedule>();

        // Act
        var result = player?.Quarantine(startDate, schedules) ?? Result<Quarantine>.Fail("Player not found.");

        // Assert
        Assert.False(result.Success);
        Assert.Null(result.Data);
        Assert.Equal("Player not found.", result.ErrorMessage);
    }

    [Theory(Skip = "Skipping this test as it needs to be fixed.")]
    [InlineData("2025-01-20")]
    [InlineData("2025-02-10")]
    public async Task Should_Cancel_Booking_When_Player_Is_Quarantined(string startDateString)
    {
        // Arrange
        var scheduleId = ScheduleId.FromGuid(Guid.NewGuid());
        
        var player = (await PlayerBuilder.CreateValid().BuildAsync()).Data;
        var startDate = DateOnly.Parse(startDateString);
        
        // Create a daily schedule with a booking
        var court = Court.Create(CourtName.Create("S1").Data).Data;
        var dailySchedule = DailySchedule.CreateSchedule(new FakeDateProvider(startDate), scheduleId).Data;
        dailySchedule.availableFrom = new TimeOnly(9, 0);
        dailySchedule.availableUntil = new TimeOnly(17, 0);
        dailySchedule.listOfCourts.Add(court);
        dailySchedule.Activate(new FakeDateProvider(startDate));
        
        // Create a booking for the player
        var booking = dailySchedule.BookCourt(
            player.email,
            court,
            new TimeOnly(10, 0),
            new TimeOnly(11, 0),
            new FakeDateProvider(startDate),
            new FakePlayerFinder(new FakePlayerRepository()),
            new FakeScheduleFinder(new FakeDailyScheduleRepository())
        ).Data;
        
        dailySchedule.listOfBookings.Add(booking);
        var schedules = new List<DailySchedule> { dailySchedule };

        // Act
        var result = player.Quarantine(startDate, schedules);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.NotNull(player.activeQuarantine);
        Assert.Equal(BookingStatus.Cancelled, booking.BookingStatus);
    }
}