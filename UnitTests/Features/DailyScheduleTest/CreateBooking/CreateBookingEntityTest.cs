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
    
    [Fact]
    public void Should_Not_Create_Booking_When_Schedule_Is_Not_Found()
    {
        // Arrange
        var scheduleFinder = new FakeScheduleFinder();
        var playerFinder = new FakePlayerFinder();

        var court = Court.Create(CourtName.Create("S1").Data);
        var email = Email.Create("test@via.dk").Data;
        var scheduleId = Guid.NewGuid();

        // Act
        var result = Booking.Create(scheduleId, court, new TimeOnly(10, 0), new TimeOnly(11, 0), email, scheduleFinder, playerFinder);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(ErrorMessage.ScheduleNotFound()._message, result.ErrorMessage);
    }
    
    [Fact]
    public async Task Should_Create_Booking_When_Field_Is_Valid()
    {
        // Arrange
        var scheduleFinder = new FakeScheduleFinder();
        var playerFinder = new FakePlayerFinder();

        var court = Court.Create(CourtName.Create("S1").Data);
        var email = Email.Create("test@via.dk").Data;
        var fakeDateProvider = new FakeDateProvider(DateOnly.FromDateTime(DateTime.Today));
        
        var emailChecker = new FakeUniqueEmailChecker();
        emailChecker.AddEmail(Email.Create("ford@via.dk").Data.Value);
        var fullName = FullName.Create("John", "Doe");
        var profileUri = ProfileUri.Create("http://example.com");
        
        var dailySchedule = DailySchedule.CreateSchedule(fakeDateProvider);
        dailySchedule.Data.availableFrom = new TimeOnly(9,0);
        dailySchedule.Data.availableUntil = new TimeOnly(17,0);

        scheduleFinder.AddSchedule(dailySchedule.Data);
        
        dailySchedule.Data.listOfCourts.Add(court);
        dailySchedule.Data.AddAvailableCourt(court, fakeDateProvider,scheduleFinder);
        
        var scheduleId= dailySchedule.Data.Id;
        var player = await Player.Register(email, fullName.Data, profileUri.Data,emailChecker);

        playerFinder.AddPlayer(player.Data);
        dailySchedule.Data.Activate(fakeDateProvider);
        
        //Act
        var result = Booking.Create(scheduleId, court, new TimeOnly(10, 0), new TimeOnly(11, 0), email, scheduleFinder, playerFinder);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Data);
    }
    
    [Fact]
    public void Should_Not_Create_Booking_When_StartTime_Before_ScheduleStart()
    {
        var scheduleFinder = new FakeScheduleFinder();
        var playerFinder = new FakePlayerFinder();
        var fakeDateProvider = new FakeDateProvider(DateOnly.FromDateTime(DateTime.Today));

        var court = Court.Create(CourtName.Create("S1").Data);
        var email = Email.Create("test@via.dk").Data;
        var dailySchedule = DailySchedule.CreateSchedule(new FakeDateProvider(DateOnly.FromDateTime(DateTime.Today))).Data;
        dailySchedule.availableFrom = new TimeOnly(9, 0);
        scheduleFinder.AddSchedule(dailySchedule);
        
        dailySchedule.listOfCourts.Add(court);
        dailySchedule.Activate(fakeDateProvider);

        var result = Booking.Create(dailySchedule.Id, court, new TimeOnly(8, 0), new TimeOnly(10, 0), email, scheduleFinder, playerFinder);

        Assert.False(result.Success);
        Assert.Equal(ErrorMessage.BookingStartTimeBeforeScheduleStartTime()._message, result.ErrorMessage);
    }
    
    [Fact]
    public void Should_Not_Create_Booking_When_EndTime_After_ScheduleEnd()
    {
        var scheduleFinder = new FakeScheduleFinder();
        var playerFinder = new FakePlayerFinder();
        var fakeDateProvider = new FakeDateProvider(DateOnly.FromDateTime(DateTime.Today));

        var court = Court.Create(CourtName.Create("S1").Data);
        var email = Email.Create("test@via.dk").Data;
        var dailySchedule = DailySchedule.CreateSchedule(fakeDateProvider).Data;
        dailySchedule.availableUntil = new TimeOnly(17, 0);
        scheduleFinder.AddSchedule(dailySchedule);
        
        dailySchedule.listOfCourts.Add(court);
        dailySchedule.Activate(fakeDateProvider);

        var result = Booking.Create(dailySchedule.Id, court, new TimeOnly(16, 0), new TimeOnly(18, 0), email, scheduleFinder, playerFinder);

        Assert.False(result.Success);
        Assert.Equal(ErrorMessage.BookingEndTimeAfterScheduleEndTime()._message, result.ErrorMessage);
    }
    
    [Fact]
    public async Task Should_Fail_When_Booking_Start_Time_After_Schedule_End_Time()
    {
        // Setup
        var scheduleFinder = new FakeScheduleFinder();
        var playerFinder = new FakePlayerFinder();
        var fakeDateProvider = new FakeDateProvider(DateOnly.FromDateTime(DateTime.Today));

        var email = Email.Create("test@via.dk").Data;
        var emailChecker = new FakeUniqueEmailChecker();
        var fullName = FullName.Create("John", "Doe");
        var profileUri = ProfileUri.Create("http://example.com");

        var court = Court.Create(CourtName.Create("S1").Data);
        var dailySchedule = DailySchedule.CreateSchedule(fakeDateProvider).Data;
        dailySchedule.availableFrom = new TimeOnly(9, 0);
        dailySchedule.availableUntil = new TimeOnly(17, 0);
        scheduleFinder.AddSchedule(dailySchedule);

        var player = await Player.Register(email, fullName.Data, profileUri.Data, emailChecker);
        playerFinder.AddPlayer(player.Data);
        
        dailySchedule.listOfCourts.Add(court);
        dailySchedule.Activate(fakeDateProvider);
        
        // Act - Booking starts at 18:00, after schedule end time 17:00
        var result = Booking.Create(
            dailySchedule.Id, 
            court, 
            new TimeOnly(18, 0), 
            new TimeOnly(19, 0), 
            email,
            scheduleFinder, 
            playerFinder
        );

        // Assert
        Assert.False(result.Success);
        Assert.Equal(ErrorMessage.BookingStartTimeAfterScheduleStartTime()._message, result.ErrorMessage);
    }

    [Fact]
    public async Task Should_Fail_When_Booking_End_Time_After_Schedule_End_Time()
    {
        // Setup
        var scheduleFinder = new FakeScheduleFinder();
        var playerFinder = new FakePlayerFinder();
        var fakeDateProvider = new FakeDateProvider(DateOnly.FromDateTime(DateTime.Today));

        var email = Email.Create("test@via.dk").Data;
        var emailChecker = new FakeUniqueEmailChecker();
        var fullName = FullName.Create("John", "Doe");
        var profileUri = ProfileUri.Create("http://example.com");
        
        var court = Court.Create(CourtName.Create("S1").Data);
        var dailySchedule = DailySchedule.CreateSchedule(fakeDateProvider).Data;
        dailySchedule.availableFrom = new TimeOnly(9, 0);
        dailySchedule.availableUntil = new TimeOnly(17, 0);
        scheduleFinder.AddSchedule(dailySchedule);
        
        dailySchedule.listOfCourts.Add(court);
        dailySchedule.Activate(fakeDateProvider);

        var player = await Player.Register(email, fullName.Data, profileUri.Data, emailChecker);
        playerFinder.AddPlayer(player.Data);
        
        // Act 
        var result = Booking.Create(
            dailySchedule.Id, 
            court, 
            new TimeOnly(16, 0), 
            new TimeOnly(18, 0), 
            email, 
            scheduleFinder, 
            playerFinder
        );

        // Assert
        Assert.False(result.Success);
        Assert.Equal(ErrorMessage.BookingEndTimeAfterScheduleEndTime()._message, result.ErrorMessage);
    }
    
    [Fact]
    public async Task Should_Fail_When_Player_Not_Found()
    {
        // Setup
        var scheduleFinder = new FakeScheduleFinder();
        var playerFinder = new FakePlayerFinder();

        var email = Email.Create("none@via.dk").Data;
        var court = Court.Create(CourtName.Create("S1").Data);
        var dailySchedule = DailySchedule.CreateSchedule(new FakeDateProvider(DateOnly.FromDateTime(DateTime.Today))).Data;
        dailySchedule.availableFrom = new TimeOnly(9, 0);
        dailySchedule.availableUntil = new TimeOnly(17, 0);
        scheduleFinder.AddSchedule(dailySchedule);
        
        // Act - Using an email not registered with any player
        var result = Booking.Create(
            dailySchedule.Id, 
            court, 
            new TimeOnly(10, 0), 
            new TimeOnly(12, 0), 
            email, 
            scheduleFinder, 
            playerFinder
        );

        // Assert
        Assert.False(result.Success);
    }
    
    [Fact]
    public async Task Should_Fail_When_Player_Is_Quarantined()
    {
        // Setup
        var scheduleFinder = new FakeScheduleFinder();
        var playerFinder = new FakePlayerFinder();
        var fakeDateProvider = new FakeDateProvider(DateOnly.FromDateTime(DateTime.Today));

        var email = Email.Create("test@via.dk").Data;
        var emailChecker = new FakeUniqueEmailChecker();
        var fullName = FullName.Create("John", "Doe");
        var profileUri = ProfileUri.Create("http://example.com");

        var court = Court.Create(CourtName.Create("S1").Data);
        var dailySchedule = DailySchedule.CreateSchedule(fakeDateProvider).Data;
        dailySchedule.availableFrom = new TimeOnly(9, 0);
        dailySchedule.availableUntil = new TimeOnly(17, 0);
        scheduleFinder.AddSchedule(dailySchedule);

        var player = await Player.Register(email, fullName.Data, profileUri.Data, emailChecker);
        
        // Set up quarantine that extends past the schedule date
        player.Data.isQuarantined = true;
        var quarantineResult = player.Data.Quarantine(DateOnly.FromDateTime(DateTime.Today.AddDays(-1)), new List<DailySchedule>());
        if (quarantineResult.Success)
        {
            player.Data.activeQuarantine = quarantineResult.Data; 
        }
        playerFinder.AddPlayer(player.Data);
        
        dailySchedule.listOfCourts.Add(court);
        dailySchedule.Activate(fakeDateProvider);
        
        // Act
        var result = Booking.Create(
            dailySchedule.Id, 
            court, 
            new TimeOnly(10, 0), 
            new TimeOnly(12, 0), 
            email, 
            scheduleFinder, 
            playerFinder
        );

        // Assert
        Assert.False(result.Success);
        Assert.Equal(ErrorMessage.QuarantinePlayerCannotBookCourt()._message, result.ErrorMessage);
    }
    
    [Fact]
    public async Task Should_Fail_When_Player_Is_Blacklisted()
    {
        // Setup
        var scheduleFinder = new FakeScheduleFinder();
        var playerFinder = new FakePlayerFinder();
        var fakeDateProvider = new FakeDateProvider(DateOnly.FromDateTime(DateTime.Today));

        var email = Email.Create("test@via.dk").Data;
        var emailChecker = new FakeUniqueEmailChecker();
        var fullName = FullName.Create("John", "Doe");
        var profileUri = ProfileUri.Create("http://example.com");

        var court = Court.Create(CourtName.Create("S1").Data);
        var dailySchedule = DailySchedule.CreateSchedule(fakeDateProvider).Data;
        dailySchedule.availableFrom = new TimeOnly(9, 0);
        dailySchedule.availableUntil = new TimeOnly(17, 0);
        scheduleFinder.AddSchedule(dailySchedule);

        var player = await Player.Register(email, fullName.Data, profileUri.Data, emailChecker);
        player.Data.isBlackListed = true;
        playerFinder.AddPlayer(player.Data);
        
        dailySchedule.listOfCourts.Add(court);
        dailySchedule.Activate(fakeDateProvider);
        
        // Act
        var result = Booking.Create(
            dailySchedule.Id, 
            court, 
            new TimeOnly(10, 0), 
            new TimeOnly(12, 0), 
            email,  
            scheduleFinder, 
            playerFinder
        );

        // Assert
        Assert.False(result.Success);
        Assert.Equal(ErrorMessage.PlayerIsBlacklisted()._message, result.ErrorMessage);
    }
   

    [Fact]
    public async Task Should_Fail_When_Booking_Creates_Gap_With_Schedule_Start_Time()
    {
        // Setup
        var scheduleFinder = new FakeScheduleFinder();
        var playerFinder = new FakePlayerFinder();
        var fakeDateProvider = new FakeDateProvider(DateOnly.FromDateTime(DateTime.Today));

        var email = Email.Create("test@via.dk").Data;
        var emailChecker = new FakeUniqueEmailChecker();
        var fullName = FullName.Create("John", "Doe");
        var profileUri = ProfileUri.Create("http://example.com");
        
        var court = Court.Create(CourtName.Create("S1").Data);
        var dailySchedule = DailySchedule.CreateSchedule(fakeDateProvider).Data;
        dailySchedule.availableFrom = new TimeOnly(9, 0);
        dailySchedule.availableUntil = new TimeOnly(17, 0);
        scheduleFinder.AddSchedule(dailySchedule);

        var player = await Player.Register(email, fullName.Data, profileUri.Data, emailChecker);
        playerFinder.AddPlayer(player.Data);
        
        dailySchedule.listOfCourts.Add(court);
        dailySchedule.Activate(fakeDateProvider);
        
        // Act - Try to book with 30 minute gap from schedule start (9:30-11:00)
        var result = Booking.Create(
            dailySchedule.Id, 
            court, 
            new TimeOnly(9, 30), 
            new TimeOnly(11, 0), 
            email, 
            scheduleFinder, 
            playerFinder
        );

        // Assert
        Assert.False(result.Success);
        Assert.Equal(ErrorMessage.OneHourGapBetweenScheduleStartTimeAndBookingStartTime()._message, result.ErrorMessage);
    }

    [Fact]
    public async Task Should_Fail_When_Booking_Creates_Gap_With_Schedule_End_Time()
    {
        // Setup
        var scheduleFinder = new FakeScheduleFinder();
        var playerFinder = new FakePlayerFinder();
        var fakeDateProvider = new FakeDateProvider(DateOnly.FromDateTime(DateTime.Today));

        var email = Email.Create("test@via.dk").Data;
        var emailChecker = new FakeUniqueEmailChecker();
        var fullName = FullName.Create("John", "Doe");
        var profileUri = ProfileUri.Create("http://example.com");
        
        var court = Court.Create(CourtName.Create("S1").Data);
        var dailySchedule = DailySchedule.CreateSchedule(new FakeDateProvider(DateOnly.FromDateTime(DateTime.Today))).Data;
        dailySchedule.availableFrom = new TimeOnly(9, 0);
        dailySchedule.availableUntil = new TimeOnly(17, 0);
        scheduleFinder.AddSchedule(dailySchedule);

        var player = await Player.Register(email, fullName.Data, profileUri.Data, emailChecker);
        playerFinder.AddPlayer(player.Data);
        
        dailySchedule.listOfCourts.Add(court);
        dailySchedule.Activate(fakeDateProvider);
        
        // Act - Try to book with 30 minute gap from schedule end (15:00-16:30)
        var result = Booking.Create(
            dailySchedule.Id, 
            court, 
            new TimeOnly(15, 0), 
            new TimeOnly(16, 30), 
            email, 
            scheduleFinder, 
            playerFinder
        );

        // Assert
        Assert.False(result.Success);
        Assert.Equal(ErrorMessage.OneHourGapBetweenScheduleEndTimeAndBookingEndTime()._message, result.ErrorMessage);
    }
    
    [Fact]
    public async Task Should_Fail_When_Booking_Creates_Gap_Less_Than_One_Hour_After()
    {
        // Setup
        var scheduleFinder = new FakeScheduleFinder();
        var playerFinder = new FakePlayerFinder();
        var fakeDateProvider = new FakeDateProvider(DateOnly.FromDateTime(DateTime.Today));

        var email = Email.Create("test@via.dk").Data;
        var emailChecker = new FakeUniqueEmailChecker();
        var fullName = FullName.Create("John", "Doe");
        var profileUri = ProfileUri.Create("http://example.com");
        
        var court = Court.Create(CourtName.Create("S1").Data);
        
        var dailySchedule = DailySchedule.CreateSchedule(fakeDateProvider).Data;
        dailySchedule.availableFrom = new TimeOnly(9, 0);
        dailySchedule.availableUntil = new TimeOnly(17, 0);
        dailySchedule.listOfCourts.Add(court);
        
        scheduleFinder.AddSchedule(dailySchedule);

        var player = await Player.Register(email, fullName.Data, profileUri.Data, emailChecker);
        playerFinder.AddPlayer(player.Data);
        
        dailySchedule.Activate(fakeDateProvider);
        
        // Create an existing booking
        var existingBooking = Booking.Create(
            dailySchedule.Id, 
            court, 
            new TimeOnly(12, 0), 
            new TimeOnly(14, 0), 
            email, 
            scheduleFinder, 
            playerFinder
        );
        dailySchedule.listOfBookings.Add(existingBooking.Data);
        
        // Act
        var result = Booking.Create(
            dailySchedule.Id, 
            court, 
            new TimeOnly(9, 0), 
            new TimeOnly(11, 30), 
            email, 
            scheduleFinder, 
            playerFinder
        );

        // Assert
        Assert.False(result.Success);
        Assert.Equal(ErrorMessage.OneHourGapShouldBeAfterAnotherBooking()._message, result.ErrorMessage);
    }
    
    [Fact]
    public async Task Should_Fail_When_Booking_Creates_Gap_Less_Than_One_Hour_Before()
    {
        // Setup
        var scheduleFinder = new FakeScheduleFinder();
        var playerFinder = new FakePlayerFinder();
        var fakeDateProvider = new FakeDateProvider(DateOnly.FromDateTime(DateTime.Today));

        var email = Email.Create("test@via.dk").Data;
        var emailChecker = new FakeUniqueEmailChecker();
        var fullName = FullName.Create("John", "Doe");
        var profileUri = ProfileUri.Create("http://example.com");
        
        var court = Court.Create(CourtName.Create("S1").Data);
        var dailySchedule = DailySchedule.CreateSchedule(new FakeDateProvider(DateOnly.FromDateTime(DateTime.Today))).Data;
        dailySchedule.availableFrom = new TimeOnly(9, 0);
        dailySchedule.availableUntil = new TimeOnly(17, 0);
        scheduleFinder.AddSchedule(dailySchedule);
        
        dailySchedule.listOfCourts.Add(court);
        
        dailySchedule.Activate(fakeDateProvider);

        var player = await Player.Register(email, fullName.Data, profileUri.Data, emailChecker);
        playerFinder.AddPlayer(player.Data);
        
        // Create an existing booking
        var existingBooking = Booking.Create(
            dailySchedule.Id, 
            court, 
            new TimeOnly(10, 0), 
            new TimeOnly(12, 0), 
            email, 
            scheduleFinder, 
            playerFinder
        );
            dailySchedule.listOfBookings.Add(existingBooking.Data);
        
        // Act 
        var result = Booking.Create(
            dailySchedule.Id, 
            court, 
            new TimeOnly(12, 30), 
            new TimeOnly(14, 0), 
            email, 
            scheduleFinder, 
            playerFinder
        );

        // Assert
        Assert.False(result.Success);
        Assert.Equal(ErrorMessage.OneHourGapShouldBeBeforeNewBooking()._message, result.ErrorMessage);
    }
    
    [Fact]
    public async Task Should_Fail_When_NonVIP_Player_Books_During_VIP_Time()
    {
        // Setup
        var scheduleFinder = new FakeScheduleFinder();
        var playerFinder = new FakePlayerFinder();
        var fakeDateProvider = new FakeDateProvider(DateOnly.FromDateTime(DateTime.Today));

        var email = Email.Create("test@via.dk").Data;
        var emailChecker = new FakeUniqueEmailChecker();
        var fullName = FullName.Create("John", "Doe");
        var profileUri = ProfileUri.Create("http://example.com");

        var court = Court.Create(CourtName.Create("S1").Data);
        var dailySchedule = DailySchedule.CreateSchedule(fakeDateProvider).Data;
        dailySchedule.availableFrom = new TimeOnly(9, 0);
        dailySchedule.availableUntil = new TimeOnly(17, 0);
        
        dailySchedule.listOfCourts.Add(court);
        dailySchedule.Activate(fakeDateProvider);
        
        // Add VIP time range
        dailySchedule.AddVipTimeSlots(new TimeOnly(11, 0), new TimeOnly(13, 0));
        scheduleFinder.AddSchedule(dailySchedule);

        var player = await Player.Register(email, fullName.Data, profileUri.Data, emailChecker);
        playerFinder.AddPlayer(player.Data);
        
        // Act 
        var result = Booking.Create(
            dailySchedule.Id, 
            court, 
            new TimeOnly(10, 0), 
            new TimeOnly(12, 0), 
            email, 
            scheduleFinder, 
            playerFinder
        );

        // Assert
        Assert.False(result.Success);
        Assert.Equal(ErrorMessage.NonVipMemberCannotBookInVipTimeSlot()._message, result.ErrorMessage);
    }
    
    [Fact]
    public async Task Should_Not_Create_Booking_When_It_Overlaps_With_Existing_Booking()
    {
        var scheduleFinder = new FakeScheduleFinder();
        var playerFinder = new FakePlayerFinder();
        var fakeDateProvider = new FakeDateProvider(DateOnly.FromDateTime(DateTime.Today));

        var email = Email.Create("test@via.dk").Data;
        var emailChecker = new FakeUniqueEmailChecker();
        emailChecker.AddEmail(Email.Create("ford@via.dk").Data.Value);
        var fullName = FullName.Create("John", "Doe");
        var profileUri = ProfileUri.Create("http://example.com");

        var court = Court.Create(CourtName.Create("S1").Data);
        var dailySchedule = DailySchedule.CreateSchedule(fakeDateProvider).Data;
        dailySchedule.availableFrom = new TimeOnly(9,0);
        dailySchedule.availableUntil = new TimeOnly(17,0);
        scheduleFinder.AddSchedule(dailySchedule);
        
        dailySchedule.listOfCourts.Add(court);
        dailySchedule.Activate(fakeDateProvider);

        var player = await Player.Register(email, fullName.Data, profileUri.Data,emailChecker);

        playerFinder.AddPlayer(player.Data);
        
        var existingBooking = Booking.Create(dailySchedule.Id, court, new TimeOnly(10, 0), new TimeOnly(12, 0), email, scheduleFinder, playerFinder);
        dailySchedule.listOfBookings.Add(existingBooking.Data);

        var result = Booking.Create(dailySchedule.Id, court, new TimeOnly(11, 0), new TimeOnly(12, 0), email, scheduleFinder, playerFinder);

        Assert.False(result.Success);
        Assert.Equal(ErrorMessage.BookingCannotBeOverlapped()._message, result.ErrorMessage);
    }
}