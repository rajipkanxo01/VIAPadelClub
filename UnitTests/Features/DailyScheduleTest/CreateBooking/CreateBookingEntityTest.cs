using UnitTests.Features.Helpers;
using VIAPadelClub.Core.Domain.Aggregates.DailySchedules;
using VIAPadelClub.Core.Domain.Aggregates.DailySchedules.Entities;
using VIAPadelClub.Core.Domain.Aggregates.DailySchedules.Values;
using VIAPadelClub.Core.Domain.Aggregates.Players;
using VIAPadelClub.Core.Domain.Aggregates.Players.Values;
using VIAPadelClub.Core.Tools.OperationResult;
using Xunit;

namespace UnitTests.Features.DailyScheduleTest.CreateBooking;

public class CreateBookingEntityTest
{
    private readonly FakeScheduleFinder _scheduleFinder = new FakeScheduleFinder();
    private readonly FakePlayerFinder _playerFinder = new FakePlayerFinder();
    private readonly FakeDateProvider _dateProvider = new FakeDateProvider(DateOnly.FromDateTime(DateTime.Today));
    
    [Fact]
    public void Should_Not_Create_Booking_When_Schedule_Is_Not_Found()
    {
        // Arrange
        var court = Court.Create(CourtName.Create("S1").Data);
        var email = Email.Create("test@via.dk").Data;
        var scheduleId = Guid.NewGuid();

        // Act
        var result = Booking.Create(scheduleId, court, new TimeOnly(10, 0), new TimeOnly(11, 0), email, _scheduleFinder, _playerFinder);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(ErrorMessage.ScheduleNotFound()._message, result.ErrorMessage);
    }
    
    [Fact]
    public async Task Should_Create_Booking_When_Field_Is_Valid()
    {
        // Arrange
        var court = Court.Create(CourtName.Create("S1").Data);
        var email = Email.Create("test@via.dk").Data;
        
        var emailChecker = new FakeUniqueEmailChecker();
        emailChecker.AddEmail(Email.Create("ford@via.dk").Data.Value);
        var fullName = FullName.Create("John", "Doe");
        var profileUri = ProfileUri.Create("http://example.com");
        
        var dailySchedule = DailySchedule.CreateSchedule(_dateProvider);
        dailySchedule.Data.availableFrom = new TimeOnly(9,0);
        dailySchedule.Data.availableUntil = new TimeOnly(17,0);

        _scheduleFinder.AddSchedule(dailySchedule.Data);
        
        dailySchedule.Data.listOfCourts.Add(court);
        dailySchedule.Data.AddAvailableCourt(court, _dateProvider,_scheduleFinder);
        
        var scheduleId= dailySchedule.Data.Id;
        var player = await Player.Register(email, fullName.Data, profileUri.Data,emailChecker);

        _playerFinder.AddPlayer(player.Data);
        dailySchedule.Data.Activate(_dateProvider);
        
        //Act
        var result = Booking.Create(scheduleId, court, new TimeOnly(10, 0), new TimeOnly(11, 0), email, _scheduleFinder, _playerFinder);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Data);
    }
    
    [Fact]
    public void Should_Not_Create_Booking_When_StartTime_Before_ScheduleStart()
    {
        var court = Court.Create(CourtName.Create("S1").Data);
        var email = Email.Create("test@via.dk").Data;
        var dailySchedule = DailySchedule.CreateSchedule(new FakeDateProvider(DateOnly.FromDateTime(DateTime.Today))).Data;
        dailySchedule.availableFrom = new TimeOnly(9, 0);
        _scheduleFinder.AddSchedule(dailySchedule);
        
        dailySchedule.listOfCourts.Add(court);
        dailySchedule.Activate(_dateProvider);

        var result = Booking.Create(dailySchedule.Id, court, new TimeOnly(8, 0), new TimeOnly(10, 0), email, _scheduleFinder, _playerFinder);

        Assert.False(result.Success);
        Assert.Equal(ErrorMessage.BookingStartTimeBeforeScheduleStartTime()._message, result.ErrorMessage);
    }
    
    [Fact]
    public void Should_Not_Create_Booking_When_EndTime_After_ScheduleEnd()
    {
        var court = Court.Create(CourtName.Create("S1").Data);
        var email = Email.Create("test@via.dk").Data;
        var dailySchedule = DailySchedule.CreateSchedule(_dateProvider).Data;
        dailySchedule.availableUntil = new TimeOnly(17, 0);
        _scheduleFinder.AddSchedule(dailySchedule);
        
        dailySchedule.listOfCourts.Add(court);
        dailySchedule.Activate(_dateProvider);

        var result = Booking.Create(dailySchedule.Id, court, new TimeOnly(16, 0), new TimeOnly(18, 0), email, _scheduleFinder, _playerFinder);

        Assert.False(result.Success);
        Assert.Equal(ErrorMessage.BookingEndTimeAfterScheduleEndTime()._message, result.ErrorMessage);
    }
    
    [Fact]
    public async Task Should_Fail_When_Booking_Start_Time_After_Schedule_End_Time()
    {
        // Setup
        var email = Email.Create("test@via.dk").Data;
        var emailChecker = new FakeUniqueEmailChecker();
        var fullName = FullName.Create("John", "Doe");
        var profileUri = ProfileUri.Create("http://example.com");

        var court = Court.Create(CourtName.Create("S1").Data);
        var dailySchedule = DailySchedule.CreateSchedule(_dateProvider).Data;
        dailySchedule.availableFrom = new TimeOnly(9, 0);
        dailySchedule.availableUntil = new TimeOnly(17, 0);
        _scheduleFinder.AddSchedule(dailySchedule);

        var player = await Player.Register(email, fullName.Data, profileUri.Data, emailChecker);
        _playerFinder.AddPlayer(player.Data);
        
        dailySchedule.listOfCourts.Add(court);
        dailySchedule.Activate(_dateProvider);
        
        // Act - Booking starts at 18:00, after schedule end time 17:00
        var result = Booking.Create(
            dailySchedule.Id, 
            court, 
            new TimeOnly(18, 0), 
            new TimeOnly(19, 0), 
            email,
            _scheduleFinder, 
            _playerFinder
        );

        // Assert
        Assert.False(result.Success);
        Assert.Equal(ErrorMessage.BookingStartTimeAfterScheduleStartTime()._message, result.ErrorMessage);
    }

    [Fact]
    public async Task Should_Fail_When_Booking_End_Time_After_Schedule_End_Time()
    {
        // Setup
        var email = Email.Create("test@via.dk").Data;
        var emailChecker = new FakeUniqueEmailChecker();
        var fullName = FullName.Create("John", "Doe");
        var profileUri = ProfileUri.Create("http://example.com");
        
        var court = Court.Create(CourtName.Create("S1").Data);
        var dailySchedule = DailySchedule.CreateSchedule(_dateProvider).Data;
        dailySchedule.availableFrom = new TimeOnly(9, 0);
        dailySchedule.availableUntil = new TimeOnly(17, 0);
        _scheduleFinder.AddSchedule(dailySchedule);
        
        dailySchedule.listOfCourts.Add(court);
        dailySchedule.Activate(_dateProvider);

        var player = await Player.Register(email, fullName.Data, profileUri.Data, emailChecker);
        _playerFinder.AddPlayer(player.Data);
        
        // Act 
        var result = Booking.Create(
            dailySchedule.Id, 
            court, 
            new TimeOnly(16, 0), 
            new TimeOnly(18, 0), 
            email, 
            _scheduleFinder, 
            _playerFinder
        );

        // Assert
        Assert.False(result.Success);
        Assert.Equal(ErrorMessage.BookingEndTimeAfterScheduleEndTime()._message, result.ErrorMessage);
    }
    
    [Fact]
    public async Task Should_Fail_When_Player_Not_Found()
    {
        // Setup
        var email = Email.Create("none@via.dk").Data;
        var court = Court.Create(CourtName.Create("S1").Data);
        var dailySchedule = DailySchedule.CreateSchedule(new FakeDateProvider(DateOnly.FromDateTime(DateTime.Today))).Data;
        dailySchedule.availableFrom = new TimeOnly(9, 0);
        dailySchedule.availableUntil = new TimeOnly(17, 0);
        _scheduleFinder.AddSchedule(dailySchedule);
        
        // Act - Using an email not registered with any player
        var result = Booking.Create(
            dailySchedule.Id, 
            court, 
            new TimeOnly(10, 0), 
            new TimeOnly(12, 0), 
            email, 
            _scheduleFinder, 
            _playerFinder
        );

        // Assert
        Assert.False(result.Success);
    }
    
    [Fact]
    public async Task Should_Fail_When_Player_Is_Quarantined()
    {
        // Setup
        var email = Email.Create("test@via.dk").Data;
        var emailChecker = new FakeUniqueEmailChecker();
        var fullName = FullName.Create("John", "Doe");
        var profileUri = ProfileUri.Create("http://example.com");

        var court = Court.Create(CourtName.Create("S1").Data);
        var dailySchedule = DailySchedule.CreateSchedule(_dateProvider).Data;
        dailySchedule.availableFrom = new TimeOnly(9, 0);
        dailySchedule.availableUntil = new TimeOnly(17, 0);
        _scheduleFinder.AddSchedule(dailySchedule);

        var player = await Player.Register(email, fullName.Data, profileUri.Data, emailChecker);
        
        // Set up quarantine that extends past the schedule date
        player.Data.isQuarantined = true;
        var quarantineResult = player.Data.Quarantine(DateOnly.FromDateTime(DateTime.Today.AddDays(-1)), new List<DailySchedule>());
        if (quarantineResult.Success)
        {
            player.Data.activeQuarantine = quarantineResult.Data; 
        }
        _playerFinder.AddPlayer(player.Data);
        
        dailySchedule.listOfCourts.Add(court);
        dailySchedule.Activate(_dateProvider);
        
        // Act
        var result = Booking.Create(
            dailySchedule.Id, 
            court, 
            new TimeOnly(10, 0), 
            new TimeOnly(12, 0), 
            email, 
            _scheduleFinder, 
            _playerFinder
        );

        // Assert
        Assert.False(result.Success);
        Assert.Equal(ErrorMessage.QuarantinePlayerCannotBookCourt()._message, result.ErrorMessage);
    }
    
    [Fact]
    public async Task Should_Fail_When_Player_Is_Blacklisted()
    {
        // Setup
        var email = Email.Create("test@via.dk").Data;
        var emailChecker = new FakeUniqueEmailChecker();
        var fullName = FullName.Create("John", "Doe");
        var profileUri = ProfileUri.Create("http://example.com");

        var court = Court.Create(CourtName.Create("S1").Data);
        var dailySchedule = DailySchedule.CreateSchedule(_dateProvider).Data;
        dailySchedule.availableFrom = new TimeOnly(9, 0);
        dailySchedule.availableUntil = new TimeOnly(17, 0);
        _scheduleFinder.AddSchedule(dailySchedule);

        var player = await Player.Register(email, fullName.Data, profileUri.Data, emailChecker);
        player.Data.isBlackListed = true;
        _playerFinder.AddPlayer(player.Data);
        
        dailySchedule.listOfCourts.Add(court);
        dailySchedule.Activate(_dateProvider);
        
        // Act
        var result = Booking.Create(
            dailySchedule.Id, 
            court, 
            new TimeOnly(10, 0), 
            new TimeOnly(12, 0), 
            email,  
            _scheduleFinder, 
            _playerFinder
        );

        // Assert
        Assert.False(result.Success);
        Assert.Equal(ErrorMessage.PlayerIsBlacklisted()._message, result.ErrorMessage);
    }
   

    [Fact]
    public async Task Should_Fail_When_Booking_Creates_Gap_With_Schedule_Start_Time()
    {
        // Setup
        var email = Email.Create("test@via.dk").Data;
        var emailChecker = new FakeUniqueEmailChecker();
        var fullName = FullName.Create("John", "Doe");
        var profileUri = ProfileUri.Create("http://example.com");
        
        var court = Court.Create(CourtName.Create("S1").Data);
        var dailySchedule = DailySchedule.CreateSchedule(_dateProvider).Data;
        dailySchedule.availableFrom = new TimeOnly(9, 0);
        dailySchedule.availableUntil = new TimeOnly(17, 0);
        _scheduleFinder.AddSchedule(dailySchedule);

        var player = await Player.Register(email, fullName.Data, profileUri.Data, emailChecker);
        _playerFinder.AddPlayer(player.Data);
        
        dailySchedule.listOfCourts.Add(court);
        dailySchedule.Activate(_dateProvider);
        
        // Act - Try to book with 30 minute gap from schedule start (9:30-11:00)
        var result = Booking.Create(
            dailySchedule.Id, 
            court, 
            new TimeOnly(9, 30), 
            new TimeOnly(11, 0), 
            email, 
            _scheduleFinder, 
            _playerFinder
        );

        // Assert
        Assert.False(result.Success);
        Assert.Equal(ErrorMessage.OneHourGapBetweenScheduleStartTimeAndBookingStartTime()._message, result.ErrorMessage);
    }

    [Fact]
    public async Task Should_Fail_When_Booking_Creates_Gap_With_Schedule_End_Time()
    {
        // Setup
        var email = Email.Create("test@via.dk").Data;
        var emailChecker = new FakeUniqueEmailChecker();
        var fullName = FullName.Create("John", "Doe");
        var profileUri = ProfileUri.Create("http://example.com");
        
        var court = Court.Create(CourtName.Create("S1").Data);
        var dailySchedule = DailySchedule.CreateSchedule(new FakeDateProvider(DateOnly.FromDateTime(DateTime.Today))).Data;
        dailySchedule.availableFrom = new TimeOnly(9, 0);
        dailySchedule.availableUntil = new TimeOnly(17, 0);
        _scheduleFinder.AddSchedule(dailySchedule);

        var player = await Player.Register(email, fullName.Data, profileUri.Data, emailChecker);
        _playerFinder.AddPlayer(player.Data);
        
        dailySchedule.listOfCourts.Add(court);
        dailySchedule.Activate(_dateProvider);
        
        // Act - Try to book with 30 minute gap from schedule end (15:00-16:30)
        var result = Booking.Create(
            dailySchedule.Id, 
            court, 
            new TimeOnly(15, 0), 
            new TimeOnly(16, 30), 
            email, 
            _scheduleFinder, 
            _playerFinder
        );

        // Assert
        Assert.False(result.Success);
        Assert.Equal(ErrorMessage.OneHourGapBetweenScheduleEndTimeAndBookingEndTime()._message, result.ErrorMessage);
    }
    
    [Fact]
    public async Task Should_Fail_When_Booking_Creates_Gap_Less_Than_One_Hour_After()
    {
        // Setup
        var email = Email.Create("test@via.dk").Data;
        var emailChecker = new FakeUniqueEmailChecker();
        var fullName = FullName.Create("John", "Doe");
        var profileUri = ProfileUri.Create("http://example.com");
        
        var email2 = Email.Create("two@via.dk").Data;
        var fullName2 = FullName.Create("Jane", "Smith");
        var profileUri2 = ProfileUri.Create("http://exampleB.com");
        
        var court = Court.Create(CourtName.Create("S1").Data);
        
        var dailySchedule = DailySchedule.CreateSchedule(_dateProvider).Data;
        dailySchedule.availableFrom = new TimeOnly(9, 0);
        dailySchedule.availableUntil = new TimeOnly(17, 0);
        dailySchedule.listOfCourts.Add(court);
        
        _scheduleFinder.AddSchedule(dailySchedule);

        var player = await Player.Register(email, fullName.Data, profileUri.Data, emailChecker);
        var player2 = await Player.Register(email2, fullName2.Data, profileUri2.Data, emailChecker);

        _playerFinder.AddPlayer(player.Data);
        _playerFinder.AddPlayer(player2.Data);

        dailySchedule.Activate(_dateProvider);
        
        // Create an existing booking
        var existingBooking = Booking.Create(
            dailySchedule.Id, 
            court, 
            new TimeOnly(12, 0), 
            new TimeOnly(14, 0), 
            email, 
            _scheduleFinder, 
            _playerFinder
        );
        dailySchedule.listOfBookings.Add(existingBooking.Data);
        
        // Act
        var result = Booking.Create(
            dailySchedule.Id, 
            court, 
            new TimeOnly(9, 0), 
            new TimeOnly(11, 30), 
            email2, 
            _scheduleFinder, 
            _playerFinder
        );

        // Assert
        Assert.False(result.Success);
        Assert.Equal(ErrorMessage.OneHourGapShouldBeAfterAnotherBooking()._message, result.ErrorMessage);
    }
    
    [Fact]
    public async Task Should_Fail_When_Booking_Creates_Gap_Less_Than_One_Hour_Before()
    {
        // Setup
        var email = Email.Create("test@via.dk").Data;
        var emailChecker = new FakeUniqueEmailChecker();
        var fullName = FullName.Create("John", "Doe");
        var profileUri = ProfileUri.Create("http://example.com");
        
        var email2 = Email.Create("two@via.dk").Data;
        var fullName2 = FullName.Create("Jane", "Smith");
        var profileUri2 = ProfileUri.Create("http://exampleB.com");

        var court = Court.Create(CourtName.Create("S1").Data);
        var dailySchedule = DailySchedule.CreateSchedule(new FakeDateProvider(DateOnly.FromDateTime(DateTime.Today))).Data;
        dailySchedule.availableFrom = new TimeOnly(9, 0);
        dailySchedule.availableUntil = new TimeOnly(17, 0);
        
        dailySchedule.listOfCourts.Add(court);
        
        _scheduleFinder.AddSchedule(dailySchedule);

        var player1 = await Player.Register(email, fullName.Data, profileUri.Data, emailChecker);
        var player2 = await Player.Register(email2, fullName2.Data, profileUri2.Data, emailChecker);
    
        _playerFinder.AddPlayer(player1.Data);
        _playerFinder.AddPlayer(player2.Data);
        
        dailySchedule.Activate(_dateProvider);
        
        // Create an existing booking
        var existingBooking = Booking.Create(
            dailySchedule.Id, 
            court, 
            new TimeOnly(10, 0), 
            new TimeOnly(12, 0), 
            email, 
            _scheduleFinder, 
            _playerFinder
        );
            dailySchedule.listOfBookings.Add(existingBooking.Data);
        
        // Act 
        var result = Booking.Create(
            dailySchedule.Id, 
            court, 
            new TimeOnly(12, 30), 
            new TimeOnly(14, 0), 
            email2, 
            _scheduleFinder, 
            _playerFinder
        );

        // Assert
        Assert.False(result.Success);
        Assert.Equal(ErrorMessage.OneHourGapShouldBeBeforeNewBooking()._message, result.ErrorMessage);
    }
    
    [Fact]
    public async Task Should_Fail_When_NonVIP_Player_Books_During_VIP_Time()
    {
        // Setup
        var email = Email.Create("test@via.dk").Data;
        var emailChecker = new FakeUniqueEmailChecker();
        var fullName = FullName.Create("John", "Doe");
        var profileUri = ProfileUri.Create("http://example.com");

        var court = Court.Create(CourtName.Create("S1").Data);
        var dailySchedule = DailySchedule.CreateSchedule(_dateProvider).Data;
        dailySchedule.availableFrom = new TimeOnly(9, 0);
        dailySchedule.availableUntil = new TimeOnly(17, 0);
        
        dailySchedule.listOfCourts.Add(court);
        dailySchedule.Activate(_dateProvider);
        
        // Add VIP time range
        dailySchedule.AddVipTimeSlots(new TimeOnly(11, 0), new TimeOnly(13, 0));
        _scheduleFinder.AddSchedule(dailySchedule);

        var player = await Player.Register(email, fullName.Data, profileUri.Data, emailChecker);
        _playerFinder.AddPlayer(player.Data);
        
        // Act 
        var result = Booking.Create(dailySchedule.Id, court, new TimeOnly(10, 0), new TimeOnly(12, 0), email, _scheduleFinder, _playerFinder);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(ErrorMessage.NonVipMemberCannotBookInVipTimeSlot()._message, result.ErrorMessage);
    }
    
    [Fact]
    public async Task Should_Not_Create_Booking_When_It_Overlaps_With_Existing_Booking()
    {
        var email = Email.Create("test@via.dk").Data;
        var emailChecker = new FakeUniqueEmailChecker();
        emailChecker.AddEmail(Email.Create("ford@via.dk").Data.Value);
        var fullName = FullName.Create("John", "Doe");
        var profileUri = ProfileUri.Create("http://example.com");

        var court = Court.Create(CourtName.Create("S1").Data);
        var dailySchedule = DailySchedule.CreateSchedule(_dateProvider).Data;
        dailySchedule.availableFrom = new TimeOnly(9,0);
        dailySchedule.availableUntil = new TimeOnly(17,0);
        _scheduleFinder.AddSchedule(dailySchedule);
        
        dailySchedule.listOfCourts.Add(court);
        dailySchedule.Activate(_dateProvider);

        var player = await Player.Register(email, fullName.Data, profileUri.Data,emailChecker);

        _playerFinder.AddPlayer(player.Data);
        
        var existingBooking = Booking.Create(dailySchedule.Id, court, new TimeOnly(10, 0), new TimeOnly(12, 0), email, _scheduleFinder, _playerFinder);
        dailySchedule.listOfBookings.Add(existingBooking.Data);

        var result = Booking.Create(dailySchedule.Id, court, new TimeOnly(11, 0), new TimeOnly(12, 0), email, _scheduleFinder, _playerFinder);

        Assert.False(result.Success);
        Assert.Equal(ErrorMessage.BookingCannotBeOverlapped()._message, result.ErrorMessage);
    }
    
    [Fact]
    public void F1_Should_Not_Create_Booking_When_Schedule_Is_Deleted()
    { 
        FakeTimeProvider timeProvider = new FakeTimeProvider(TimeOnly.FromDateTime(DateTime.Now));

        // Arrange
        var dailySchedule = DailySchedule.CreateSchedule(_dateProvider).Data!;
        dailySchedule.DeleteSchedule(_dateProvider, timeProvider);
        _scheduleFinder.AddSchedule(dailySchedule);

        var email = Email.Create("test@via.dk").Data!;
        var court = Court.Create(CourtName.Create("S1").Data);

        // Act
        var result = Booking.Create(dailySchedule.Id, court, new TimeOnly(10, 0), new TimeOnly(11, 0), email, _scheduleFinder, _playerFinder);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(ErrorMessage.ScheduleNotActive()._message, result.ErrorMessage);
    }
    
    [Fact]
    public void F2_Should_Not_Create_Booking_When_Schedule_Is_Draft()
    {
        // Arrange
        var dailySchedule = DailySchedule.CreateSchedule(_dateProvider).Data!;
        _scheduleFinder.AddSchedule(dailySchedule);

        var email = Email.Create("test@via.dk").Data!;
        var court = Court.Create(CourtName.Create("S1").Data);

        // Act
        var result = Booking.Create(dailySchedule.Id, court, new TimeOnly(10, 0), new TimeOnly(11, 0), email, _scheduleFinder, _playerFinder);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(ErrorMessage.ScheduleNotActive()._message, result.ErrorMessage);
    }
    
    [Fact]
    public async Task F4_Should_Not_Create_Booking_When_Court_Not_Found()
    {
        // Arrange
        var email = Email.Create("test@via.dk").Data!;
        var emailChecker = new FakeUniqueEmailChecker();
        var fullName = FullName.Create("John", "Doe");
        var profileUri = ProfileUri.Create("http://example.com");

        var court = Court.Create(CourtName.Create("S1").Data!);
        var nonExistentCourt = Court.Create(CourtName.Create("Non-Existent Court").Data);
        var dailySchedule = DailySchedule.CreateSchedule(_dateProvider).Data;
        dailySchedule.availableFrom = new TimeOnly(9, 0);
        dailySchedule.availableUntil = new TimeOnly(17, 0);
        dailySchedule.listOfCourts.Add(court);
        _scheduleFinder.AddSchedule(dailySchedule);
        dailySchedule.Activate(_dateProvider);
        
        var player = await Player.Register(email, fullName.Data, profileUri.Data, emailChecker);
        _playerFinder.AddPlayer(player.Data);

        // Act
        var result = Booking.Create(dailySchedule.Id, nonExistentCourt, new TimeOnly(10, 0), new TimeOnly(11, 0), email, _scheduleFinder, _playerFinder);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(ErrorMessage.CourtDoesntExistInSchedule()._message, result.ErrorMessage);
    }
    
    [Fact]
    public async Task F9_Should_Not_Create_Booking_With_Invalid_Time_Format()
    {
        // Arrange
        var email = Email.Create("test@via.dk").Data!;
        var emailChecker = new FakeUniqueEmailChecker();
        var fullName = FullName.Create("John", "Doe");
        var profileUri = ProfileUri.Create("http://example.com");

        var court = Court.Create(CourtName.Create("S1").Data!);
        var dailySchedule = DailySchedule.CreateSchedule(_dateProvider).Data;
        dailySchedule.availableFrom = new TimeOnly(9, 0);
        dailySchedule.availableUntil = new TimeOnly(17, 0);
        dailySchedule.listOfCourts.Add(court);
        _scheduleFinder.AddSchedule(dailySchedule);
        dailySchedule.Activate(_dateProvider);
        
        var player = await Player.Register(email, fullName.Data, profileUri.Data, emailChecker);
        _playerFinder.AddPlayer(player.Data);

        // Act
        var result = Booking.Create(dailySchedule.Id, court, new TimeOnly(10, 2), new TimeOnly(11, 9), email, _scheduleFinder, _playerFinder);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(ErrorMessage.InvalidBookingTimeSpan()._message, result.ErrorMessage);
    }
    
    [Fact]
    public async Task F10_Should_Not_Create_Booking_If_Duration_Is_Less_Than_One_Hour()
    {
        // Arrange
        var email = Email.Create("test@via.dk").Data!;
        var emailChecker = new FakeUniqueEmailChecker();
        var fullName = FullName.Create("John", "Doe");
        var profileUri = ProfileUri.Create("http://example.com");

        var court = Court.Create(CourtName.Create("S1").Data);
        var dailySchedule = DailySchedule.CreateSchedule(_dateProvider).Data;
        dailySchedule.availableFrom = new TimeOnly(9, 0);
        dailySchedule.availableUntil = new TimeOnly(17, 0);

        _scheduleFinder.AddSchedule(dailySchedule);
        dailySchedule.listOfCourts.Add(court);
        dailySchedule.Activate(_dateProvider);

        var player = await Player.Register(email, fullName.Data, profileUri.Data, emailChecker);
        _playerFinder.AddPlayer(player.Data);

        // Act
        var result = Booking.Create(
        dailySchedule.Id, 
        court, 
        new TimeOnly(10, 0), 
        new TimeOnly(10, 30),
        email, 
        _scheduleFinder, 
        _playerFinder
        );

        // Assert
        Assert.False(result.Success);
        Assert.Equal(ErrorMessage.BookingDurationError()._message, result.ErrorMessage);
    }
    
    [Fact]
    public async Task F12_Should_Not_Create_Booking_If_Duration_Exceeds_Three_Hours()
    {
        // Arrange
        var email = Email.Create("test@via.dk").Data!;
        var emailChecker = new FakeUniqueEmailChecker();
        var fullName = FullName.Create("John", "Doe");
        var profileUri = ProfileUri.Create("http://example.com");

        var court = Court.Create(CourtName.Create("S1").Data);
        var dailySchedule = DailySchedule.CreateSchedule(_dateProvider).Data;
        dailySchedule.availableFrom = new TimeOnly(9, 0);
        dailySchedule.availableUntil = new TimeOnly(17, 0);
        dailySchedule.listOfCourts.Add(court);
    
        _scheduleFinder.AddSchedule(dailySchedule);
        dailySchedule.Activate(_dateProvider);

        var player = await Player.Register(email, fullName.Data, profileUri.Data, emailChecker);
        _playerFinder.AddPlayer(player.Data);

        // Act
        var result = Booking.Create(dailySchedule.Id, court, new TimeOnly(10, 0), new TimeOnly(14, 30), email, _scheduleFinder, _playerFinder);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(ErrorMessage.BookingDurationError()._message, result.ErrorMessage);
    }

    [Fact]
    public async Task F17_Should_Not_Create_Booking_When_Player_Already_Has_A_Booking_On_Same_Day()
    {
        // Arrange
        var email = Email.Create("test@via.dk").Data!;
        var emailChecker = new FakeUniqueEmailChecker();
        var fullName = FullName.Create("John", "Doe");
        var profileUri = ProfileUri.Create("http://example.com");

        var court = Court.Create(CourtName.Create("S1").Data!);
        var dailySchedule = DailySchedule.CreateSchedule(_dateProvider).Data;
        dailySchedule.availableFrom = new TimeOnly(9, 0);
        dailySchedule.availableUntil = new TimeOnly(17, 0);
        dailySchedule.listOfCourts.Add(court);
    
        _scheduleFinder.AddSchedule(dailySchedule);
        dailySchedule.Activate(_dateProvider);

        var player = await Player.Register(email, fullName.Data, profileUri.Data, emailChecker);
        _playerFinder.AddPlayer(player.Data);

        var firstBooking = Booking.Create(dailySchedule.Id, court, new TimeOnly(10, 0), new TimeOnly(11, 0), email, _scheduleFinder, _playerFinder);
        dailySchedule.listOfbookings.Add(firstBooking.Data);
        
        var result = Booking.Create(dailySchedule.Id, court, new TimeOnly(14, 0), new TimeOnly(15, 0), email, _scheduleFinder, _playerFinder);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(ErrorMessage.BookingLimitExceeded()._message, result.ErrorMessage);
    }
    
    [Fact]
    public async Task S1_Should_Create_Booking_When_Valid()
    {
        // Arrange
        var email = Email.Create("test@via.dk").Data!;
        var emailChecker = new FakeUniqueEmailChecker();
        var fullName = FullName.Create("John", "Doe");
        var profileUri = ProfileUri.Create("http://example.com");

        var court = Court.Create(CourtName.Create("S1").Data);
        var dailySchedule = DailySchedule.CreateSchedule(_dateProvider).Data;
        dailySchedule.availableFrom = new TimeOnly(9, 0);
        dailySchedule.availableUntil = new TimeOnly(17, 0);

        _scheduleFinder.AddSchedule(dailySchedule);
        dailySchedule.listOfCourts.Add(court);
        dailySchedule.Activate(_dateProvider);

        var player = await Player.Register(email, fullName.Data, profileUri.Data, emailChecker);
        _playerFinder.AddPlayer(player.Data);

        var startTime = new TimeOnly(10, 0);
        var endTime = new TimeOnly(11, 0);

        var result = Booking.Create(dailySchedule.Id, court, startTime, endTime, email, _scheduleFinder, _playerFinder);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.Equal(email.Value, result.Data!.BookedBy.Value);
        Assert.Equal(startTime, result.Data.StartTime);
        Assert.Equal(endTime, result.Data.EndTime);
    }
}


