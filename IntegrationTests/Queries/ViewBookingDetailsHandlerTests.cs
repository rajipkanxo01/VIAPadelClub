using IntegrationTests.Seeders;
using Microsoft.EntityFrameworkCore;
using VIAPadelClub.Core.QueryContracts.Queries;
using VIAPadelClub.Infrastructure.EfcQueries.GeneratedModels;
using VIAPadelClub.Infrastructure.EfcQueries.Queries;
using Xunit;

namespace IntegrationTests.Queries;

public class ViewBookingDetailsHandlerTests
{
    private static VeadatabaseProductionContext CreateSeededContext()
    {
        var path = Path.Combine(Path.GetTempPath(), $"test_{Guid.NewGuid()}.db");
        var options = new DbContextOptionsBuilder<VeadatabaseProductionContext>()
            .UseSqlite($"Data Source={path}")
            .Options;

        var context = new VeadatabaseProductionContext(options);
        
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