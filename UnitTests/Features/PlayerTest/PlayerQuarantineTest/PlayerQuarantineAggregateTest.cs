namespace UnitTests.Features.PlayerTest.PlayerQuarantineTest;

using VIAPadelClub.Core.Domain.Aggregates.DailySchedules;
using VIAPadelClub.Core.Domain.Aggregates.DailySchedules.Entities;
using VIAPadelClub.Core.Domain.Aggregates.Players;
using VIAPadelClub.Core.Domain.Aggregates.Players.Entities;
using VIAPadelClub.Core.Tools.OperationResult;
using Xunit;

public class PlayerQuarantineTest {
    
    [Theory]
    [InlineData("test@via.dk", "Pramesh", "Smith", "http://profile.uri", "2025-01-20")]
    [InlineData("jane@via.dk", "Rajip", "Kang", "http://profile.uri", "2025-02-10")]
    public void Should_Quarantine_Player_Successfully(string email, string firstName, string lastName, string profileUri, string startDateString)
    {
        // Arrange
        var player = Player.Register(email, firstName, lastName, profileUri).Data!;
        var startDate = DateOnly.Parse(startDateString);
        var schedules = new List<DailySchedule>();

        // Act
        var result = player.Quarantine(startDate, schedules);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.Single(player.quarantines); // Only one quarantine should exist
        Assert.Equal(startDate, result.Data!.StartDate);
        Assert.Equal(startDate.AddDays(3), result.Data!.EndDate);
    }
    
    [Theory]
    [InlineData("test@via.dk", "Pramesh", "Smith", "http://profile.uri", "2025-01-20")]
    [InlineData("jane@via.dk", "Rajip", "Kang", "http://profile.uri", "2025-02-10")]
    public void Should_Extend_Existing_Quarantine(string email, string firstName, string lastName, string profileUri, string startDateString)
    {
        // Arrange
        var player = Player.Register(email, firstName, lastName, profileUri).Data!;
        var startDate = DateOnly.Parse(startDateString);
        var schedules = new List<DailySchedule>();


        // First Quarantine
        player.Quarantine(startDate, schedules);

        // Act - Quarantine again on the same date, should extend
        var result = player.Quarantine(startDate, schedules);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.Single(player.quarantines); // No new quarantine, now we extended
        Assert.Equal(startDate, result.Data.StartDate);
        Assert.Equal(startDate.AddDays(6), result.Data.EndDate); // Extended by another 3 days
    }
    
    [Theory]
    [InlineData("test@via.dk", "John", "Doe", "http://profile.uri", "2025-01-20")]
    [InlineData("jane@via.dk", "Jane", "Smith", "http://profile.uri", "2025-02-10")]
    public void Should_Fail_To_Quarantine_Blacklisted_Player(string email, string firstName, string lastName, string profileUri, string startDateString)
    {
        // Arrange
        var player = Player.Register(email, firstName, lastName, profileUri).Data!;
        player.isBlackListed = true;
        var startDate = DateOnly.Parse(startDateString);
        var schedules = new List<DailySchedule>();


        // Act
        var result = player.Quarantine(startDate, schedules);

        // Assert
        Assert.False(result.Success);
        Assert.Null(result.Data);
        Assert.Equal("Player is already blacklisted and cannot be quarantined.", result.ErrorMessage);
        Assert.Empty(player.quarantines);
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
    
    //TODO: Following - To be uncommented once we have booking entity created.
    
    // [Fact]
    // public void Should_Cancel_Bookings_During_Quarantine()
    // {
    //     // Arrange
    //     var player = Player.Register("testing@via.dk", "Troles", "Larsen", "http://profile.uri").Data!;
    //     
    //     var schedule1 = DailySchedule.CreateSchedule();
    //     var schedule2 = DailySchedule.CreateSchedule();
    //
    //     schedule1.scheduleDate = new DateTime(2025, 1, 20);
    //     schedule2.scheduleDate = new DateTime(2025, 1, 21);
    //
    //     var booking1 = new Booking(Guid.NewGuid(), player, new Court(new CourtName("Court A")), schedule1.scheduleDate, new TimeSpan(2, 0, 0));
    //     var booking2 = new Booking(Guid.NewGuid(), player, new Court(new CourtName("Court B")), schedule2.scheduleDate, new TimeSpan(1, 30, 0));
    //
    //     schedule1.listOfBookings.Add(booking1);
    //     schedule2.listOfBookings.Add(booking2);
    //
    //     var schedules = new List<DailySchedule> { schedule1, schedule2 };
    //
    //     // Act
    //     var quarantineResult = player.Quarantine(new DateOnly(2025, 1, 20), schedules);
    //
    //     // Assert
    //     Assert.True(quarantineResult.Success);
    //     Assert.NotNull(quarantineResult.Data);
    //     
    //     // Check that the bookings were canceled
    //     Assert.DoesNotContain(schedule1.listOfBookings, b => b.bookedBy.email == player.email);
    //     Assert.DoesNotContain(schedule2.listOfBookings, b => b.bookedBy.email == player.email);
    // }
    
    
    //TODO: - Once we have the booking entity. Add a test to verify that the booking that fall outside of the quarentine period aren't cancelled. 

}