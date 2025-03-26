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

namespace UnitTests.Features.DailyScheduleTest.CreateBooking;

public class CreateBookingAggregateTest
{
    private readonly FakeDailyScheduleRepository dailyScheduleRepository;
    private readonly FakePlayerRepository playerRepository;
    private readonly FakeScheduleFinder _scheduleFinder;
    private readonly FakePlayerFinder _playerFinder;
    private readonly FakeDateProvider _dateProvider = new FakeDateProvider(DateOnly.FromDateTime(DateTime.Today));
    
    public CreateBookingAggregateTest()
    {
        dailyScheduleRepository = new FakeDailyScheduleRepository();
        playerRepository = new FakePlayerRepository();
        _scheduleFinder = new FakeScheduleFinder(dailyScheduleRepository);
        _playerFinder = new FakePlayerFinder(playerRepository);
    }
    
    [Fact]
    public void Should_Not_Create_Booking_When_Schedule_Is_Not_Found()
    {
        // Arrange
        var court = Court.Create(CourtName.Create("S1").Data);
        var email = Email.Create("test@via.dk").Data;
        var dailySchedule = DailySchedule.CreateSchedule(_dateProvider);

        // Act
        var result = dailySchedule.Data.BookCourt(email,court.Data, new TimeOnly(10, 0), new TimeOnly(11, 0),_dateProvider, _playerFinder,_scheduleFinder);

        // Assert
        Assert.False(result.Success);
    }
    
    [Fact]
    public async Task Should_Create_Booking_When_Field_Is_Valid()
    {
        // Arrange
        var court = Court.Create(CourtName.Create("S1").Data);
        
        var player = (await PlayerBuilder.CreateValid().BuildAsync()).Data;
        
        var dailySchedule = DailySchedule.CreateSchedule(_dateProvider);
        dailySchedule.Data.availableFrom = new TimeOnly(9,0);
        dailySchedule.Data.availableUntil = new TimeOnly(17,0);

        _scheduleFinder.AddSchedule(dailySchedule.Data);
        
        dailySchedule.Data.listOfCourts.Add(court.Data);
        dailySchedule.Data.AddAvailableCourt(court.Data, _dateProvider,_scheduleFinder);

        _playerFinder.AddPlayer(player);
        dailySchedule.Data.Activate(_dateProvider);
        
        // Act
        var result = dailySchedule.Data.BookCourt(player.email,court.Data, new TimeOnly(10, 0), new TimeOnly(11, 0),_dateProvider, _playerFinder,_scheduleFinder);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Data);
    }
    
    [Fact]
    public void Should_Not_Create_Booking_When_StartTime_Before_ScheduleStart()
    {
        //Arrange
        var court = Court.Create(CourtName.Create("S1").Data).Data;
        var email = Email.Create("test@via.dk").Data;
        var dailySchedule = DailySchedule.CreateSchedule(_dateProvider).Data;
        dailySchedule.availableFrom = new TimeOnly(9, 0);
        _scheduleFinder.AddSchedule(dailySchedule);
        
        dailySchedule.listOfCourts.Add(court);
        dailySchedule.Activate(_dateProvider);

        //Act
        var result = dailySchedule.BookCourt(email, court, new TimeOnly(8, 0), new TimeOnly(10, 0),_dateProvider, _playerFinder,_scheduleFinder);

        Assert.False(result.Success);
        Assert.Equal(DailyScheduleError.BookingStartTimeBeforeScheduleStartTime()._message, result.ErrorMessage);
    }
    
    [Fact]
    public void Should_Not_Create_Booking_When_EndTime_After_ScheduleEnd()
    {
        //Arrange
        var court = Court.Create(CourtName.Create("S1").Data).Data;
        var email = Email.Create("test@via.dk").Data;
        var dailySchedule = DailySchedule.CreateSchedule(_dateProvider).Data;
        dailySchedule.availableUntil = new TimeOnly(17, 0);
        _scheduleFinder.AddSchedule(dailySchedule);
        
        dailySchedule.listOfCourts.Add(court);
        dailySchedule.Activate(_dateProvider);
        
        // Act
        var result = dailySchedule.BookCourt(email,court, new TimeOnly(16, 0), new TimeOnly(18, 0),_dateProvider, _playerFinder,_scheduleFinder);

        //Assert
        Assert.False(result.Success);
        Assert.Equal(DailyScheduleError.BookingEndTimeAfterScheduleEndTime()._message, result.ErrorMessage);
    }
    
    [Fact]
    public async Task Should_Fail_When_Booking_Start_Time_After_Schedule_End_Time()
    {
        // Arrange
        var player = (await PlayerBuilder.CreateValid().BuildAsync());

        var court = Court.Create(CourtName.Create("S1").Data).Data;
        var dailySchedule = DailySchedule.CreateSchedule(_dateProvider).Data;
        dailySchedule.availableFrom = new TimeOnly(9, 0);
        dailySchedule.availableUntil = new TimeOnly(17, 0);
        _scheduleFinder.AddSchedule(dailySchedule);

        _playerFinder.AddPlayer(player.Data);
        
        dailySchedule.listOfCourts.Add(court);
        dailySchedule.Activate(_dateProvider);
        
        // Act
        var result = dailySchedule.BookCourt(player.Data.email,court, new TimeOnly(18, 0), new TimeOnly(19, 0),_dateProvider, _playerFinder,_scheduleFinder);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(DailyScheduleError.BookingStartTimeAfterScheduleStartTime()._message, result.ErrorMessage);
    }

    [Fact]
    public async Task Should_Fail_When_Booking_End_Time_After_Schedule_End_Time()
    {
        // Arrange
        var player = (await PlayerBuilder.CreateValid().BuildAsync()).Data;
        
        var court = Court.Create(CourtName.Create("S1").Data).Data;
        var dailySchedule = DailySchedule.CreateSchedule(_dateProvider).Data;
        dailySchedule.availableFrom = new TimeOnly(9, 0);
        dailySchedule.availableUntil = new TimeOnly(17, 0);
        _scheduleFinder.AddSchedule(dailySchedule);
        
        dailySchedule.listOfCourts.Add(court);
        dailySchedule.Activate(_dateProvider);

        _playerFinder.AddPlayer(player);
        
        // Act
        var result = dailySchedule.BookCourt(player.email,court, new TimeOnly(16, 0), new TimeOnly(18, 0),_dateProvider, _playerFinder,_scheduleFinder);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(DailyScheduleError.BookingEndTimeAfterScheduleEndTime()._message, result.ErrorMessage);
    }
    
    [Fact]
    public async Task Should_Fail_When_Player_Not_Found()
    {
        // Arrange
        var email = Email.Create("none@via.dk").Data;
        var court = Court.Create(CourtName.Create("S1").Data).Data;
        var dailySchedule = DailySchedule.CreateSchedule(_dateProvider).Data;
        dailySchedule.availableFrom = new TimeOnly(9, 0);
        dailySchedule.availableUntil = new TimeOnly(17, 0);
        _scheduleFinder.AddSchedule(dailySchedule);
        
        // Act
        var result = dailySchedule.BookCourt(email,court, new TimeOnly(10, 0), new TimeOnly(12, 0),_dateProvider, _playerFinder,_scheduleFinder);

        // Assert
        Assert.False(result.Success);
    }
    
    [Fact]
    public async Task Should_Fail_When_Player_Is_Quarantined()
    {
        // Arrange
        var player = (await PlayerBuilder.CreateValid().BuildAsync()).Data;

        var court = Court.Create(CourtName.Create("S1").Data).Data;
        var dailySchedule = DailySchedule.CreateSchedule(_dateProvider).Data;
        dailySchedule.availableFrom = new TimeOnly(9, 0);
        dailySchedule.availableUntil = new TimeOnly(17, 0);
        _scheduleFinder.AddSchedule(dailySchedule);
        
        // Set up quarantine that extends past the schedule date
        player.isQuarantined = true;
        var quarantineResult = player.Quarantine(DateOnly.FromDateTime(DateTime.Today.AddDays(-1)), new List<DailySchedule>());
        if (quarantineResult.Success)
        {
            player.activeQuarantine = quarantineResult.Data; 
        }
        _playerFinder.AddPlayer(player);
        
        dailySchedule.listOfCourts.Add(court);
        dailySchedule.Activate(_dateProvider);
        
        // Act
        var result = dailySchedule.BookCourt(player.email,court, new TimeOnly(10, 0), new TimeOnly(12, 0),_dateProvider, _playerFinder,_scheduleFinder);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(DailyScheduleError.QuarantinePlayerCannotBookCourt()._message, result.ErrorMessage);
    }
    
    [Fact]
    public async Task Should_Fail_When_Player_Is_Blacklisted()
    {
        // Arrange
        var player = (await PlayerBuilder.CreateValid().BuildAsync()).Data;

        var court = Court.Create(CourtName.Create("S1").Data).Data;
        var dailySchedule = DailySchedule.CreateSchedule(_dateProvider).Data;
        dailySchedule.availableFrom = new TimeOnly(9, 0);
        dailySchedule.availableUntil = new TimeOnly(17, 0);
        _scheduleFinder.AddSchedule(dailySchedule);

        player.isBlackListed = true;
        _playerFinder.AddPlayer(player);
        
        dailySchedule.listOfCourts.Add(court);
        dailySchedule.Activate(_dateProvider);
        
        // Act
        var result = dailySchedule.BookCourt(player.email,court, new TimeOnly(10, 0), new TimeOnly(12, 0),_dateProvider, _playerFinder,_scheduleFinder);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(DailyScheduleError.PlayerIsBlacklisted()._message, result.ErrorMessage);
    }
   

    [Fact]
    public async Task Should_Fail_When_Booking_Creates_Gap_With_Schedule_Start_Time()
    {
        // Arrange
        var player = (await PlayerBuilder.CreateValid().BuildAsync()).Data;
        
        var court = Court.Create(CourtName.Create("S1").Data).Data;
        var dailySchedule = DailySchedule.CreateSchedule(_dateProvider).Data;
        dailySchedule.availableFrom = new TimeOnly(9, 0);
        dailySchedule.availableUntil = new TimeOnly(17, 0);
        _scheduleFinder.AddSchedule(dailySchedule);

        _playerFinder.AddPlayer(player);
        
        dailySchedule.listOfCourts.Add(court);
        dailySchedule.Activate(_dateProvider);
        
        // Act
        var result = dailySchedule.BookCourt(player.email,court, new TimeOnly(9, 30), new TimeOnly(11, 0),_dateProvider, _playerFinder,_scheduleFinder);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(DailyScheduleError.OneHourGapBetweenScheduleStartTimeAndBookingStartTime()._message, result.ErrorMessage);
    }

    [Fact]
    public async Task Should_Fail_When_Booking_Creates_Gap_With_Schedule_End_Time()
    {
        // Arrange
        var player = (await PlayerBuilder.CreateValid().BuildAsync()).Data;
        
        var court = Court.Create(CourtName.Create("S1").Data).Data;
        var dailySchedule = DailySchedule.CreateSchedule(_dateProvider).Data;
        dailySchedule.availableFrom = new TimeOnly(9, 0);
        dailySchedule.availableUntil = new TimeOnly(17, 0);
        _scheduleFinder.AddSchedule(dailySchedule);

        _playerFinder.AddPlayer(player);
        
        dailySchedule.listOfCourts.Add(court);
        dailySchedule.Activate(_dateProvider);
        
        // Act
        var result = dailySchedule.BookCourt(player.email,court, new TimeOnly(15, 0), new TimeOnly(16, 30),_dateProvider, _playerFinder,_scheduleFinder);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(DailyScheduleError.OneHourGapBetweenScheduleEndTimeAndBookingEndTime()._message, result.ErrorMessage);
    }
    
    [Fact]
    public async Task Should_Fail_When_Booking_Creates_Gap_Less_Than_One_Hour_After()
    {
        // Arrange
        var player = (await PlayerBuilder.CreateValid().BuildAsync()).Data;
        var player2 = (await PlayerBuilder.CreateValid().WithEmail("test@via.dk").BuildAsync()).Data;
        
        var court = Court.Create(CourtName.Create("S1").Data).Data;
        
        var dailySchedule = DailySchedule.CreateSchedule(_dateProvider).Data;
        dailySchedule.availableFrom = new TimeOnly(9, 0);
        dailySchedule.availableUntil = new TimeOnly(17, 0);
        dailySchedule.listOfCourts.Add(court);
        
        _scheduleFinder.AddSchedule(dailySchedule);

        _playerFinder.AddPlayer(player);
        _playerFinder.AddPlayer(player2);

        dailySchedule.Activate(_dateProvider);
        
        var existingBooking = dailySchedule.BookCourt(player.email,court, new TimeOnly(12, 0), new TimeOnly(14, 0),_dateProvider, _playerFinder,_scheduleFinder);
        dailySchedule.listOfBookings.Add(existingBooking.Data);
        
        // Act
        var result = dailySchedule.BookCourt(player2.email,court, new TimeOnly(9, 0), new TimeOnly(11, 30),_dateProvider, _playerFinder,_scheduleFinder);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(DailyScheduleError.OneHourGapShouldBeAfterAnotherBooking()._message, result.ErrorMessage);
    }
    
    [Fact]
    public async Task Should_Fail_When_Booking_Creates_Gap_Less_Than_One_Hour_Before()
    {
        // Arrange
        var player = (await PlayerBuilder.CreateValid().BuildAsync()).Data;
        var player2 = (await PlayerBuilder.CreateValid().WithEmail("test@via.dk").BuildAsync()).Data;

        var court = Court.Create(CourtName.Create("S1").Data).Data;
        var dailySchedule = DailySchedule.CreateSchedule(new FakeDateProvider(DateOnly.FromDateTime(DateTime.Today))).Data;
        dailySchedule.availableFrom = new TimeOnly(9, 0);
        dailySchedule.availableUntil = new TimeOnly(17, 0);
        
        dailySchedule.listOfCourts.Add(court);
        
        _scheduleFinder.AddSchedule(dailySchedule);
    
        _playerFinder.AddPlayer(player);
        _playerFinder.AddPlayer(player2);
        
        dailySchedule.Activate(_dateProvider);
        
        // Act
        var existingBooking = dailySchedule.BookCourt(player.email,court, new TimeOnly(10, 0), new TimeOnly(12, 0),_dateProvider, _playerFinder,_scheduleFinder);
        dailySchedule.listOfBookings.Add(existingBooking.Data);
        
        // Act
        var result = dailySchedule.BookCourt(player2.email,court, new TimeOnly(12, 30), new TimeOnly(14, 0),_dateProvider, _playerFinder,_scheduleFinder);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(DailyScheduleError.OneHourGapShouldBeBeforeNewBooking()._message, result.ErrorMessage);
    }
    
    [Fact]
    public async Task Should_Fail_When_NonVIP_Player_Books_During_VIP_Time()
    {
        // Arrange
        var player = (await PlayerBuilder.CreateValid().BuildAsync()).Data;

        var court = Court.Create(CourtName.Create("S1").Data).Data;
        var dailySchedule = DailySchedule.CreateSchedule(_dateProvider).Data;
        dailySchedule.availableFrom = new TimeOnly(9, 0);
        dailySchedule.availableUntil = new TimeOnly(17, 0);
        
        dailySchedule.listOfCourts.Add(court);
        dailySchedule.Activate(_dateProvider);
        
        // Add VIP time range
        dailySchedule.AddVipTimeSlots(new TimeOnly(11, 0), new TimeOnly(13, 0));
        _scheduleFinder.AddSchedule(dailySchedule);

        _playerFinder.AddPlayer(player);
        
        // Act
        var result = dailySchedule.BookCourt(player.email,court, new TimeOnly(10, 0), new TimeOnly(12, 0),_dateProvider, _playerFinder,_scheduleFinder);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(DailyScheduleError.NonVipMemberCannotBookInVipTimeSlot()._message, result.ErrorMessage);
    }
    
    [Fact]
    public async Task Should_Not_Create_Booking_When_It_Overlaps_With_Existing_Booking()
    {
        //Arrange
        var player = (await PlayerBuilder.CreateValid().BuildAsync()).Data;

        var court = Court.Create(CourtName.Create("S1").Data).Data;
        var dailySchedule = DailySchedule.CreateSchedule(_dateProvider).Data;
        dailySchedule.availableFrom = new TimeOnly(9,0);
        dailySchedule.availableUntil = new TimeOnly(17,0);
        _scheduleFinder.AddSchedule(dailySchedule);
        
        dailySchedule.listOfCourts.Add(court);
        dailySchedule.Activate(_dateProvider);

        _playerFinder.AddPlayer(player);
        
        var existingBooking = dailySchedule.BookCourt(player.email, court, new TimeOnly(10, 0), new TimeOnly(12, 0),_dateProvider,_playerFinder, _scheduleFinder);
        dailySchedule.listOfBookings.Add(existingBooking.Data);
        
        // Act
        var result = dailySchedule.BookCourt(player.email,court, new TimeOnly(11, 0), new TimeOnly(12, 0),_dateProvider, _playerFinder,_scheduleFinder);
        
        // Assert
        Assert.False(result.Success);
        Assert.Equal(DailyScheduleError.BookingCannotBeOverlapped()._message, result.ErrorMessage);
    }
    
    [Fact]
    public void F1_Should_Not_Create_Booking_When_Schedule_Is_Deleted()
    { 
        // Arrange
        FakeTimeProvider timeProvider = new FakeTimeProvider(TimeOnly.FromDateTime(DateTime.Now));
        
        var dailySchedule = DailySchedule.CreateSchedule(_dateProvider).Data!;
        dailySchedule.DeleteSchedule(_dateProvider, timeProvider);
        _scheduleFinder.AddSchedule(dailySchedule);

        var email = Email.Create("test@via.dk").Data!;
        var court = Court.Create(CourtName.Create("S1").Data).Data;
        
        // Act
        var result = dailySchedule.BookCourt(email,court, new TimeOnly(10, 0), new TimeOnly(11, 0),_dateProvider, _playerFinder,_scheduleFinder);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(DailyScheduleError.ScheduleNotActive()._message, result.ErrorMessage);
    }
    
    [Fact]
    public void F2_Should_Not_Create_Booking_When_Schedule_Is_Draft()
    {
        // Arrange
        var dailySchedule = DailySchedule.CreateSchedule(_dateProvider).Data!;
        _scheduleFinder.AddSchedule(dailySchedule);

        var email = Email.Create("test@via.dk").Data!;
        var court = Court.Create(CourtName.Create("S1").Data).Data;
        
        // Act
        var result = dailySchedule.BookCourt(email,court, new TimeOnly(10, 0), new TimeOnly(11, 0),_dateProvider, _playerFinder,_scheduleFinder);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(DailyScheduleError.ScheduleNotActive()._message, result.ErrorMessage);
    }
    
    [Fact]
    public async Task F9_Should_Not_Create_Booking_With_Invalid_Time_Format()
    {
        // Arrange
        var player = (await PlayerBuilder.CreateValid().BuildAsync()).Data;

        var court = Court.Create(CourtName.Create("S1").Data!).Data;
        var dailySchedule = DailySchedule.CreateSchedule(_dateProvider).Data;
        dailySchedule.availableFrom = new TimeOnly(9, 0);
        dailySchedule.availableUntil = new TimeOnly(17, 0);
        
        dailySchedule.listOfCourts.Add(court);
        _scheduleFinder.AddSchedule(dailySchedule);
        dailySchedule.Activate(_dateProvider);
        
        _playerFinder.AddPlayer(player);

        // Act
        var result = dailySchedule.BookCourt(player.email,court, new TimeOnly(10, 2), new TimeOnly(11, 9),_dateProvider, _playerFinder,_scheduleFinder);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(DailyScheduleError.InvalidBookingTimeSpan()._message, result.ErrorMessage);
    }
    
    [Fact]
    public async Task F10_Should_Not_Create_Booking_If_Duration_Is_Less_Than_One_Hour()
    {
        // Arrange
        var player = (await PlayerBuilder.CreateValid().BuildAsync()).Data;

        var court = Court.Create(CourtName.Create("S1").Data).Data;
        var dailySchedule = DailySchedule.CreateSchedule(_dateProvider).Data;
        dailySchedule.availableFrom = new TimeOnly(9, 0);
        dailySchedule.availableUntil = new TimeOnly(17, 0);

        _scheduleFinder.AddSchedule(dailySchedule);
        dailySchedule.listOfCourts.Add(court);
        dailySchedule.Activate(_dateProvider);

        _playerFinder.AddPlayer(player);

        // Act
        var result = dailySchedule.BookCourt(player.email,court, new TimeOnly(10, 0), new TimeOnly(10, 30),_dateProvider, _playerFinder,_scheduleFinder);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(DailyScheduleError.BookingDurationError()._message, result.ErrorMessage);
    }
    
    [Fact]
    public async Task F12_Should_Not_Create_Booking_If_Duration_Exceeds_Three_Hours()
    {
        // Arrange
        var player = (await PlayerBuilder.CreateValid().BuildAsync()).Data;

        var court = Court.Create(CourtName.Create("S1").Data).Data;
        var dailySchedule = DailySchedule.CreateSchedule(_dateProvider).Data;
        dailySchedule.availableFrom = new TimeOnly(9, 0);
        dailySchedule.availableUntil = new TimeOnly(17, 0);
        dailySchedule.listOfCourts.Add(court);
    
        _scheduleFinder.AddSchedule(dailySchedule);
        dailySchedule.Activate(_dateProvider);

        _playerFinder.AddPlayer(player);

        // Act
        var result = dailySchedule.BookCourt(player.email,court, new TimeOnly(10, 0), new TimeOnly(14, 30),_dateProvider, _playerFinder,_scheduleFinder);
        
        // Assert
        Assert.False(result.Success);
        Assert.Equal(DailyScheduleError.BookingDurationError()._message, result.ErrorMessage);
    }

    [Fact]
    public async Task F17_Should_Not_Create_Booking_When_Player_Already_Has_A_Booking_On_Same_Day()
    {
        // Arrange
        var player = (await PlayerBuilder.CreateValid().BuildAsync()).Data;

        var court = Court.Create(CourtName.Create("S1").Data!).Data;
        var dailySchedule = DailySchedule.CreateSchedule(_dateProvider).Data;
        dailySchedule.availableFrom = new TimeOnly(9, 0);
        dailySchedule.availableUntil = new TimeOnly(17, 0);
        dailySchedule.listOfCourts.Add(court);
    
        _scheduleFinder.AddSchedule(dailySchedule);
        dailySchedule.Activate(_dateProvider);

        _playerFinder.AddPlayer(player);

        var firstBooking = dailySchedule.BookCourt(player.email,court, new TimeOnly(10, 0), new TimeOnly(11, 0),_dateProvider,_playerFinder, _scheduleFinder);
        dailySchedule.listOfBookings.Add(firstBooking.Data);
        
        // Act
        var result = dailySchedule.BookCourt(player.email,court, new TimeOnly(14, 0), new TimeOnly(15, 0),_dateProvider, _playerFinder,_scheduleFinder);
        
        // Assert
        Assert.False(result.Success);
        Assert.Equal(DailyScheduleError.BookingLimitExceeded()._message, result.ErrorMessage);
    }
    
    [Fact]
    public async Task S1_Should_Create_Booking_When_Valid()
    {
        // Arrange
        var player = (await PlayerBuilder.CreateValid().BuildAsync()).Data;

        var court = Court.Create(CourtName.Create("S1").Data).Data;
        var dailySchedule = DailySchedule.CreateSchedule(_dateProvider).Data;
        dailySchedule.availableFrom = new TimeOnly(9, 0);
        dailySchedule.availableUntil = new TimeOnly(17, 0);

        _scheduleFinder.AddSchedule(dailySchedule);
        dailySchedule.listOfCourts.Add(court);
        dailySchedule.Activate(_dateProvider);

        _playerFinder.AddPlayer(player);

        var startTime = new TimeOnly(10, 0);
        var endTime = new TimeOnly(11, 0);

        // Act
        var result = dailySchedule.BookCourt(player.email,court, startTime,endTime,_dateProvider, _playerFinder,_scheduleFinder);
        
        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.Equal(player.email.Value, result.Data!.BookedBy.Value);
        Assert.Equal(startTime, result.Data.StartTime);
        Assert.Equal(endTime, result.Data.EndTime);
    }
}


