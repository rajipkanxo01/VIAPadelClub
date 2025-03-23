using VIAPadelClub.Core.Application.CommandDispatching.Commands.Booking;
using VIAPadelClub.Core.Tools.OperationResult;
using Xunit;

namespace UnitTests.Features.DailyScheduleTest.CancelsBooking;

public class CancelsBookingCommandTest
{
    [Fact]
    public void Should_Create_When_Providing_Valid_Inputs()
    {
        // Arrange
        string bookingIdStr = Guid.NewGuid().ToString();
        string emailStr = "123456@via.dk";
        string dailyScheduleIdStr = Guid.NewGuid().ToString();

        // Act
        var result = PlayerCancelsBookingCommand.Create(bookingIdStr, emailStr, dailyScheduleIdStr);

        // Assert
        Assert.True(result.Success);
    }
    
    [Fact]
    public void Should_Fail_When_Providing_Invalid_Email()
    {
        // Arrange
        string bookingIdStr = Guid.NewGuid().ToString();
        string emailStr = "1@via.dk";
        string dailyScheduleIdStr = Guid.NewGuid().ToString();

        // Act
        var result = PlayerCancelsBookingCommand.Create(bookingIdStr, emailStr, dailyScheduleIdStr);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(ErrorMessage.InvalidEmailFormat()._message, result.ErrorMessage);
    }
}