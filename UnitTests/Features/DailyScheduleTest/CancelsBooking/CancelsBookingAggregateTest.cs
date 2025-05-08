using UnitTests.Features.Helpers;
using UnitTests.Features.Helpers.Repository;
using VIAPadelClub.Core.Domain.Aggregates.DailySchedules;
using VIAPadelClub.Core.Domain.Aggregates.DailySchedules.Contracts;
using VIAPadelClub.Core.Domain.Aggregates.DailySchedules.Entities;
using VIAPadelClub.Core.Domain.Aggregates.DailySchedules.Values;
using VIAPadelClub.Core.Domain.Aggregates.Players;
using VIAPadelClub.Core.Domain.Aggregates.Players.Contracts;
using VIAPadelClub.Core.Domain.Aggregates.Players.Values;
using VIAPadelClub.Core.Tools.OperationResult;
using Xunit;

namespace UnitTests.Features.DailyScheduleTest.CancelsBooking;

public class CancelsBookingAggregateTest
{
    private readonly FakeDailyScheduleRepository dailyScheduleRepository = new FakeDailyScheduleRepository();
    private readonly FakePlayerRepository playerRepository = new FakePlayerRepository();

    [Fact] // S1
    public void Should_Cancel_Booking_When_More_Than_One_Hour_Before_Start()
    {
        // Arrange
        var courtName = CourtName.Create("D1").Data;
        var bookingStartTime = new TimeOnly(12, 0, 0);
        var bookingEndTime = new TimeOnly(13, 0, 0);

        var cancellationDate = new DateOnly(2025, 01, 31);
        var cancellationTime = new TimeOnly(10, 30, 0);

        var fakeDateProvider = new FakeDateProvider(cancellationDate);
        var fakeTimeProvider = new FakeTimeProvider(cancellationTime);
        var fakePlayerFinder = new FakePlayerFinder(playerRepository);
        var fakeScheduleFinder = new FakeScheduleFinder(dailyScheduleRepository);

        var court = Court.Create(courtName).Data;

        var schedule = SetupDailySchedule(court, fakeScheduleFinder);
        var player = SetupPlayer("test@via.dk", "Test", "Player", fakePlayerFinder);
        
        var createdBooking = schedule.BookCourt(player.email, court, bookingStartTime, bookingEndTime, fakeDateProvider, fakePlayerFinder, fakeScheduleFinder).Data;
        schedule.listOfBookings.Add(createdBooking);
        
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
        var courtName = CourtName.Create("D1").Data;
        var scheduleDate = DateOnly.FromDateTime(DateTime.Today).AddDays(2);
        var bookingStartTime = new TimeOnly(12, 0, 0);
        var bookingEndTime = new TimeOnly(14, 0, 0);

        var cancellationDate = new DateOnly(2025, 03, 18);
        var cancellationTime = new TimeOnly(11, 59, 0);

        var fakeDateProvider = new FakeDateProvider(cancellationDate);
        var fakeTimeProvider = new FakeTimeProvider(cancellationTime);
        var fakePlayerFinder = new FakePlayerFinder(playerRepository);
        var fakeScheduleFinder = new FakeScheduleFinder(dailyScheduleRepository);

        var court = Court.Create(courtName).Data;
    
        var schedule = SetupDailySchedule(court, fakeScheduleFinder);
        var player = SetupPlayer("test@via.dk", "Test", "Player", fakePlayerFinder);

        var createdBooking = schedule.BookCourt(player.email, court, bookingStartTime, bookingEndTime, fakeDateProvider, fakePlayerFinder, fakeScheduleFinder).Data;
        schedule.listOfBookings.Add(createdBooking);

        // Act
        var cancelResult = schedule.CancelBooking(createdBooking.BookingId, fakeDateProvider, fakeTimeProvider, player.email);

        // Assert
        Assert.True(cancelResult.Success);
        Assert.Equal(BookingStatus.Cancelled, createdBooking.BookingStatus);
    }


    [Theory] // F1
    [InlineData("12:00", "12:01")] // Cancellation 1 min after start
    [InlineData("12:00", "13:00")] // Cancellation after booking started
    public void Should_Fail_When_Booking_Is_In_The_Past(string bookingStartTimeStr, string cancellationTimeStr)
    {
        // Arrange
        var courtName = CourtName.Create("D1").Data;

        var bookingStartTime = TimeOnly.Parse(bookingStartTimeStr);
        var bookingEndTime = bookingStartTime.AddHours(2);

        var cancellationDate = DateOnly.FromDateTime(DateTime.Today.AddDays(5));
        var cancellationTime = TimeOnly.Parse(cancellationTimeStr);

        var fakeDateProvider = new FakeDateProvider(cancellationDate);
        var fakeTimeProvider = new FakeTimeProvider(cancellationTime);
        var fakePlayerFinder = new FakePlayerFinder(playerRepository);
        var fakeScheduleFinder = new FakeScheduleFinder(dailyScheduleRepository);

        var court = Court.Create(courtName).Data;

        var schedule = SetupDailySchedule(court, fakeScheduleFinder);
        var player = SetupPlayer("test@via.dk", "Test", "Player", fakePlayerFinder);

        var createdBooking = schedule.BookCourt(player.email, court, bookingStartTime, bookingEndTime, fakeDateProvider, fakePlayerFinder, fakeScheduleFinder).Data;
        schedule.listOfBookings.Add(createdBooking);

        // Act
        var cancelResult = schedule.CancelBooking(createdBooking.BookingId, fakeDateProvider, fakeTimeProvider, player.email);

        // Assert
        Assert.False(cancelResult.Success);
        Assert.Equal(DailyScheduleError.CannotCancelPastBooking()._message, cancelResult.ErrorMessage);
    }

    [Theory] // F2
    [InlineData("12:00", "11:30")]
    [InlineData("12:00", "11:01")]
    [InlineData("12:00", "11:59")]
    public void Should_Fail_When_Cancellation_Is_Too_Late(string bookingStartTimeStr, string cancellationTimeStr)
    {
        // Arrange
        var courtName = CourtName.Create("D1").Data;

        var bookingStartTime = TimeOnly.Parse(bookingStartTimeStr);
        var bookingEndTime = bookingStartTime.AddHours(2);

        var cancellationDate = DateOnly.FromDateTime(DateTime.Today.AddDays(4));
        var cancellationTime = TimeOnly.Parse(cancellationTimeStr);

        var fakeDateProvider = new FakeDateProvider(cancellationDate);
        var fakeTimeProvider = new FakeTimeProvider(cancellationTime);
        var fakePlayerFinder = new FakePlayerFinder(playerRepository);
        var fakeScheduleFinder = new FakeScheduleFinder(dailyScheduleRepository);

        var court = Court.Create(courtName).Data;

        var schedule = SetupDailySchedule(court, fakeScheduleFinder);
        var player = SetupPlayer("test@via.dk", "Test", "Player", fakePlayerFinder);

        var createdBooking = schedule.BookCourt(player.email, court, bookingStartTime, bookingEndTime, fakeDateProvider, fakePlayerFinder, fakeScheduleFinder).Data;
        schedule.listOfBookings.Add(createdBooking);
        
        // Act
        var result = schedule.CancelBooking(createdBooking.BookingId, fakeDateProvider, fakeTimeProvider, player.email);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(DailyScheduleError.CancellationTooLate()._message, result.ErrorMessage);
    }

    [Fact] // F3
    public void Should_Fail_When_Cancelling_Nonexistent_Booking()
    {
        // Arrange
        var courtName = CourtName.Create("D1").Data;
        var cancellationDate = new DateOnly(2025, 01, 31);
        var cancellationTime = new TimeOnly(12, 0, 0);

        var fakeDateProvider = new FakeDateProvider(cancellationDate);
        var fakeTimeProvider = new FakeTimeProvider(cancellationTime);
        var fakePlayerFinder = new FakePlayerFinder(playerRepository);
        var fakeScheduleFinder = new FakeScheduleFinder(dailyScheduleRepository);

        var court = Court.Create(courtName).Data;

        var schedule = SetupDailySchedule(court, fakeScheduleFinder);
        var firstPlayer = SetupPlayer("111111@via.dk", "First", "Player", fakePlayerFinder);

        var nonExistentBookingId = BookingId.Create();

        // Act
        var cancelResult = schedule.CancelBooking(nonExistentBookingId, fakeDateProvider, fakeTimeProvider, firstPlayer.email);

        // Assert
        Assert.False(cancelResult.Success);
        Assert.Equal(DailyScheduleError.BookingNotFound()._message, cancelResult.ErrorMessage);
    }

    [Fact] // F5
    public void Should_Fail_When_Player_Does_Not_Own_Booking()
    {
        // Arrange
        var courtName = CourtName.Create("D1").Data;
        var scheduleDate = DateOnly.FromDateTime(DateTime.Today).AddDays(2);
        var bookingStartTime = new TimeOnly(12, 0, 0);
        var bookingEndTime = new TimeOnly(13, 0, 0);

        var cancellationDate = new DateOnly(2025, 01, 31);
        var cancellationTime = new TimeOnly(10, 30, 0);

        var fakeDateProvider = new FakeDateProvider(cancellationDate);
        var fakeTimeProvider = new FakeTimeProvider(cancellationTime);
        var fakePlayerFinder = new FakePlayerFinder(playerRepository);
        var fakeScheduleFinder = new FakeScheduleFinder(dailyScheduleRepository);

        var court = Court.Create(courtName).Data;

        var schedule = SetupDailySchedule(court, fakeScheduleFinder);
        var firstPlayer = SetupPlayer("111111@via.dk", "First", "Player", fakePlayerFinder);
        var secondPlayer = SetupPlayer("111112@via.dk", "Second", "Player", fakePlayerFinder);

        var createdBooking = schedule.BookCourt(firstPlayer.email, court, bookingStartTime, bookingEndTime, fakeDateProvider, fakePlayerFinder, fakeScheduleFinder).Data;
        schedule.listOfBookings.Add(createdBooking);

        // Act
        var result = schedule.CancelBooking(createdBooking.BookingId, fakeDateProvider, fakeTimeProvider, secondPlayer.email);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(DailyScheduleError.BookingOwnershipViolation()._message, result.ErrorMessage);
    }

    private DailySchedule SetupDailySchedule(Court court, IScheduleFinder fakeScheduleFinder)
    {
        var fakeDateProvider = new FakeDateProvider(DateOnly.FromDateTime(DateTime.Today).AddDays(3)); 
        var dateForDailySchedule = DateOnly.FromDateTime(DateTime.Today.AddDays(4));
        var scheduleId = ScheduleId.FromGuid(Guid.NewGuid());
        
        var schedule = DailySchedule.CreateSchedule(fakeDateProvider, scheduleId).Data;

        schedule.UpdateScheduleDateAndTime(dateForDailySchedule, new TimeOnly(10, 0), new TimeOnly(14, 0), fakeDateProvider);
        schedule.AddAvailableCourt(court, fakeDateProvider, fakeScheduleFinder);
        schedule.listOfCourts.Add(court);
        schedule.Activate(fakeDateProvider);
        
        fakeScheduleFinder.AddSchedule(schedule);
        return schedule;
    }

    private Player SetupPlayer(string emailStr, string firstNameStr, string lastNameStr, IPlayerFinder fakePlayerFinder)
    {
        var email = Email.Create(emailStr).Data;
        var fullName = FullName.Create(firstNameStr, lastNameStr).Data;
        var profileUri = ProfileUri.Create("http://example.com").Data;
        var fakeUniqueEmailChecker = new FakeUniqueEmailChecker();
        
        var player = Player.Register(email, fullName, profileUri, fakeUniqueEmailChecker).Result.Data;
        fakePlayerFinder.AddPlayer(player);
        
        return player;
    }
}