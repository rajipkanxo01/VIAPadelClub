using UnitTests.Features.Helpers;
using VIAPadelClub.Core.Domain.Aggregates.DailySchedules;
using VIAPadelClub.Core.Domain.Aggregates.DailySchedules.Entities;
using VIAPadelClub.Core.Domain.Aggregates.DailySchedules.Values;
using VIAPadelClub.Core.Domain.Aggregates.Players;
using VIAPadelClub.Core.Domain.Aggregates.Players.Values;
using VIAPadelClub.Core.Tools.OperationResult;
using Xunit;

namespace UnitTests.Features.DailyScheduleTest.CancelsBooking;

public class CancelsBookingAggregateTest
{
    private readonly IScheduleFinder _fakeScheduleRepo = new FakeScheduleFinder();

    [Fact] // S1
    public void Should_Cancel_Booking_When_More_Than_One_Hour_Before_Start()
    {
        // Arrange
        var courtName = "D1";
        var bookingDate = new DateOnly(2025, 01, 31);
        var bookingStartTime = new TimeOnly(12, 0, 0);
        var bookingEndTime = new TimeOnly(13, 0, 0);

        var cancellationDate = new DateOnly(2025, 01, 31);
        var cancellationTime = new TimeOnly(10, 30, 0);

        var fakeDateProvider = new FakeDateProvider(cancellationDate);
        var fakeTimeProvider = new FakeTimeProvider(cancellationTime);

        var schedule = SetupDailySchedule(courtName, bookingDate);
        var player = SetupPlayer("test@via.dk", "Test", "Player");

        var createdBooking = schedule.CreateBooking(schedule.scheduleId, courtName, bookingStartTime, bookingEndTime, bookingDate, player.email, _fakeScheduleRepo).Data;

        // Act
        var cancelResult = schedule.CancelBooking(createdBooking.BookingId, fakeDateProvider, fakeTimeProvider, player.email);

        // Assert
        Assert.True(cancelResult.Success);
        Assert.Equal(BookingStatus.Cancelled, createdBooking.BookingStatus);
    }

    [Fact] // S3
    public void Should_Cancel_Booking_When_Date_Before_Schedule_But_Time_Less_Than_1_Hour()
    {
        // Arrange
        var courtName = "D1";
        var bookingDate = new DateOnly(2025, 03, 19);
        var bookingStartTime = new TimeOnly(12, 0, 0);
        var bookingEndTime = new TimeOnly(14, 0, 0);

        var cancellationDate = new DateOnly(2025, 03, 18);
        var cancellationTime = new TimeOnly(11, 59, 0);

        var fakeDateProvider = new FakeDateProvider(cancellationDate);
        var fakeTimeProvider = new FakeTimeProvider(cancellationTime);
    
        var schedule = SetupDailySchedule(courtName, bookingDate);
        var player = SetupPlayer("test@via.dk", "Test", "Player");

        var createdBooking = schedule.CreateBooking(
            schedule.scheduleId, courtName, bookingStartTime, bookingEndTime, bookingDate, player.email, _fakeScheduleRepo).Data;

        // Act
        var cancelResult = schedule.CancelBooking(createdBooking.BookingId, fakeDateProvider, fakeTimeProvider, player.email);

        // Assert
        Assert.True(cancelResult.Success);
        Assert.Equal(BookingStatus.Cancelled, createdBooking.BookingStatus);
    }


    [Theory] // F1
    [InlineData( "12:00", "2025-03-19", "10:00")] // Cancellation the next day
    [InlineData("12:00", "2025-03-18", "12:01")] // Cancellation 1 min after start
    [InlineData("12:00", "2025-03-18", "13:00")] // Cancellation after booking started
    public void Should_Fail_When_Booking_Is_In_The_Past(string bookingStartTimeStr, string cancellationDateStr, string cancellationTimeStr)
    {
        // Arrange
        var courtName = "D1";

        var bookingDate = DateOnly.Parse("2025-03-18");
        var bookingStartTime = TimeOnly.Parse(bookingStartTimeStr);
        var bookingEndTime = bookingStartTime.AddHours(2);

        var cancellationDate = DateOnly.Parse(cancellationDateStr);
        var cancellationTime = TimeOnly.Parse(cancellationTimeStr);

        var fakeDateProvider = new FakeDateProvider(cancellationDate);
        var fakeTimeProvider = new FakeTimeProvider(cancellationTime);

        var schedule = SetupDailySchedule(courtName, bookingDate);
        var player = SetupPlayer("test@via.dk", "Test", "Player");

        var createdBooking = schedule.CreateBooking(schedule.scheduleId, courtName, bookingStartTime, bookingEndTime, bookingDate, player.email, _fakeScheduleRepo).Data;

        // Act
        var cancelResult = schedule.CancelBooking(createdBooking.BookingId, fakeDateProvider, fakeTimeProvider, player.email);

        // Assert
        Assert.False(cancelResult.Success);
        Assert.Equal(ErrorMessage.CannotCancelPastBooking()._message, cancelResult.ErrorMessage);
    }

    [Theory] // F2
    [InlineData("12:00", "11:30")]
    [InlineData("12:00", "11:01")]
    [InlineData("12:00", "11:59")]
    public void Should_Fail_When_Cancellation_Is_Too_Late(string bookingStartTimeStr, string cancellationTimeStr)
    {
        // Arrange
        var courtName = "D1";

        var bookingDate = DateOnly.Parse("2025-03-18");
        var bookingStartTime = TimeOnly.Parse(bookingStartTimeStr);
        var bookingEndTime = bookingStartTime.AddHours(2);

        var cancellationDate = DateOnly.Parse("2025-03-18");
        var cancellationTime = TimeOnly.Parse(cancellationTimeStr);

        var fakeDateProvider = new FakeDateProvider(cancellationDate);
        var fakeTimeProvider = new FakeTimeProvider(cancellationTime);

        var schedule = SetupDailySchedule(courtName, bookingDate);
        var player = SetupPlayer("test@via.dk", "Test", "Player");

        var createdBooking = schedule.CreateBooking(schedule.scheduleId, courtName, bookingStartTime, bookingEndTime, bookingDate, player.email, _fakeScheduleRepo).Data;
        
        // Act
        var result = schedule.CancelBooking(createdBooking.BookingId, fakeDateProvider, fakeTimeProvider, player.email);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(ErrorMessage.CancellationTooLate()._message, result.ErrorMessage);
    }

    [Fact] // F3
    public void Should_Fail_When_Cancelling_Nonexistent_Booking()
    {
        // Arrange
        var courtName = "D1";
        var cancellationDate = new DateOnly(2025, 01, 31);
        var cancellationTime = new TimeOnly(12, 0, 0);

        var fakeDateProvider = new FakeDateProvider(cancellationDate);
        var fakeTimeProvider = new FakeTimeProvider(cancellationTime);

        var schedule = SetupDailySchedule(courtName, cancellationDate);
        var firstPlayer = SetupPlayer("111111@via.dk", "First", "Player");

        var nonExistentBookingId = Guid.NewGuid();

        // Act
        var cancelResult = schedule.CancelBooking(nonExistentBookingId, fakeDateProvider, fakeTimeProvider, firstPlayer.email);

        // Assert
        Assert.False(cancelResult.Success);
        Assert.Equal(ErrorMessage.BookingNotFound()._message, cancelResult.ErrorMessage);
    }

    [Fact] // F5
    public void Should_Fail_When_Player_Does_Not_Own_Booking()
    {
        // Arrange
        var courtName = "D1";
        var bookingDate = new DateOnly(2025, 01, 31);
        var bookingStartTime = new TimeOnly(12, 0, 0);
        var bookingEndTime = new TimeOnly(13, 0, 0);

        var cancellationDate = new DateOnly(2025, 01, 31);
        var cancellationTime = new TimeOnly(10, 30, 0);

        var fakeDateProvider = new FakeDateProvider(cancellationDate);
        var fakeTimeProvider = new FakeTimeProvider(cancellationTime);

        var schedule = SetupDailySchedule(courtName, bookingDate);
        var firstPlayer = SetupPlayer("111111@via.dk", "First", "Player");
        var secondPlayer = SetupPlayer("111112@via.dk", "Second", "Player");

        var createdBooking = schedule.CreateBooking(schedule.scheduleId, courtName, bookingStartTime, bookingEndTime, bookingDate, firstPlayer.email, _fakeScheduleRepo).Data;

        // Act
        var result = schedule.CancelBooking(createdBooking.BookingId, fakeDateProvider, fakeTimeProvider, secondPlayer.email);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(ErrorMessage.BookingOwnershipViolation()._message, result.ErrorMessage);
    }

    private DailySchedule SetupDailySchedule(string courtName, DateOnly scheduleDate)
    {
        var schedule = DailySchedule.CreateSchedule(new FakeDateProvider(scheduleDate)).Data;

        schedule.UpdateScheduleDateAndTime(scheduleDate, new TimeOnly(10, 0), new TimeOnly(14, 0), new FakeDateProvider(scheduleDate));
        schedule.Activate(new FakeDateProvider(scheduleDate));
        schedule.AddAvailableCourt(Court.Create(CourtName.Create(courtName).Data), new FakeDateProvider(scheduleDate), _fakeScheduleRepo);
        return schedule;
    }

    private Player SetupPlayer(string email, string firstName, string lastName)
    {
        return Player.Register(Email.Create(email).Data, FullName.Create(firstName, lastName).Data, ProfileUri.Create("http://example.com").Data, new FakeUniqueEmailChecker()).Result.Data;
    }
}