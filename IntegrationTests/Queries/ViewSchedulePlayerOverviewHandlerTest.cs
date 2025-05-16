using IntegrationTests.Helpers;
using VIAPadelClub.Core.QueryContracts.Queries;
using VIAPadelClub.Infrastructure.EfcQueries.Queries;
using Xunit;
using Xunit.Abstractions;

namespace IntegrationTests.Queries;

public class ViewSchedulePlayerOverviewHandlerTest(ITestOutputHelper output)
{
    [Fact]
    public async Task HandleAsync_ValidScheduleId_ReturnsScheduleData()
    {
        // Arrange
        var context = MyDbContext.CreateSeededContext();
        var handler = new ViewSchedulePlayerOverviewQueryHandler(context);
        var query = new ViewSchedulePlayerOverview.Query("2025-04-09");

        // Act
        var result = await handler.HandleAsync(query);

        // Assert
        //Assert.True(result.Success);
        Assert.True(result.Success, $"Expected success but got error: {result.ErrorMessage}");
        //Assert.NotNull(result.Data);

        var data = result.Data;
         Assert.False(string.IsNullOrWhiteSpace(data.Date));
        
        var allSchedules = context.DailySchedules.ToList();
        output.WriteLine($"Seeded Schedules: {string.Join(", ", allSchedules.Select(s => s.ScheduleDate))}");
        
        foreach (var s in allSchedules)
        {
            output.WriteLine($"Schedule: Id={s.ScheduleId}, Date={s.ScheduleDate}, Status={s.Status}");
        }
        
        
        Assert.All(data.AvailableCourts, court =>
        {
            output.WriteLine($"Court: {court.Name}");
            Assert.False(string.IsNullOrWhiteSpace(court.Name));
        });
        
        Assert.All(data.VipTimeRanges, range =>
        {
            output.WriteLine($"VIP Time: {range.Start} - {range.End}");
            Assert.False(string.IsNullOrWhiteSpace(range.Start));
        });
        
        Assert.All(data.Bookings, booking =>
        {
            output.WriteLine($"Booking: {booking.StartTime}-{booking.EndTime} by {booking.PlayerEmail}");
            Assert.False(string.IsNullOrWhiteSpace(booking.PlayerEmail));
        });
    }

    [Fact]
    public async Task HandleAsync_InvalidScheduleId_ReturnsFailure()
    {
        // Arrange
        var context = MyDbContext.CreateSeededContext();
        var handler = new ViewSchedulePlayerOverviewQueryHandler(context);
        var date = DateTime.Now.ToString("yyyy-MM-dd");
        var query = new ViewSchedulePlayerOverview.Query(date);

        // Act
        var result = await handler.HandleAsync(query);

        // Assert
        Assert.False(result.Success);
        Assert.Contains("not found", result.ErrorMessage, StringComparison.OrdinalIgnoreCase);
    }
}