using VIAPadelClub.Core.Application.CommandDispatching.Commands.DailySchedule;
using VIAPadelClub.Core.Tools.OperationResult;
using Xunit;

namespace UnitTests.Features.DailyScheduleTest.CreateBooking;

public class CreateBookingCommandTest
{

    [Fact]
    public void Should_Create_Booking_When_Valid_Input_Is_Provided()
    {
        //Arrange
        string scheduleIdStr = Guid.NewGuid().ToString();
        string bookedByEmail = "test@via.dk";
        string courtName = "s1";
        string bookingStartTime = "9:00";
        string bookingEndTime = "12:00";
        
        //Act
        var result = CreateBookingCommand.Create(scheduleIdStr, bookedByEmail, bookingStartTime, bookingEndTime,courtName);
        
        //Assert
        Assert.True(result.Success);
    }

    [Fact]
    public void Should_Not_Create_Booking_When_Invalid_courtName_Is_Not_Provided()
    {
        //Arrange
        string scheduleIdStr = Guid.NewGuid().ToString();
        string bookedByEmail = "test@via.dk";
        string courtName = "c1";
        string bookingStartTime = "9:00";
        string bookingEndTime = "12:00";
        
        //Act
        var result = CreateBookingCommand.Create(scheduleIdStr, bookedByEmail, bookingStartTime, bookingEndTime,courtName);
        
        //Assert
        Assert.False(result.Success);
        Assert.Equal(DailyScheduleError.InvalidStartingLetter()._message, result.ErrorMessage);
    }

}