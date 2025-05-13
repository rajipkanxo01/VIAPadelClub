using Xunit;
using VIAPadelClub.Infrastructure.EfcQueries.GeneratedModels;
using VIAPadelClub.Infrastructure.EfcQueries.Queries;
using VIAPadelClub.Core.QueryContracts.Queries;
using Microsoft.EntityFrameworkCore;
using IntegrationTests.Seeders;

public class ViewBookingDetailsHandlerTests
{
    private static VeadatabaseProductionContext CreateSeededContext()
    {
        var context = new VeadatabaseProductionContext();
        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();
        context.SeedTestData();
        context.ChangeTracker.Clear();
        return context;
    }

    [Fact]
    public async Task ReturnsBookingDetails_WhenBookingExists()
    {
        // Arrange
        using var context = CreateSeededContext();
        var booking = context.Bookings.AsNoTracking().FirstOrDefault();
        Assert.NotNull(booking);

        var handler = new ViewBookingDetailsHandler(context);

        // Act
        var result = await handler.HandleAsync(new ViewBookingDetails.Query(Guid.Parse(booking.BookingId)));

        // Assert
        Assert.NotNull(result);
        Assert.Equal(Guid.Parse(booking.BookingId), result.BookingId);
        Assert.Equal(booking.BookedBy, result.BookedBy);
        Assert.Equal(booking.Name, result.CourtName);
        Assert.Equal(booking.StartTime, result.StartTime);
        Assert.Equal(booking.EndTime, result.EndTime);
        Assert.Equal(booking.BookedDate, result.BookedDate);
        Assert.Equal(booking.BookingStatus, result.BookingStatus);
        Assert.Equal(Guid.Parse(booking.ScheduleId), result.ScheduleId);
        Assert.Equal($"{booking.Duration} Mins", result.Duration);
    }
    
    [Fact]
    public async Task Throws_WhenBookingNotFound()
    {
        using var context = CreateSeededContext();
        var handler = new ViewBookingDetailsHandler(context);
        var nonExistingId = Guid.NewGuid();

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            handler.HandleAsync(new ViewBookingDetails.Query(nonExistingId)));
    }
}