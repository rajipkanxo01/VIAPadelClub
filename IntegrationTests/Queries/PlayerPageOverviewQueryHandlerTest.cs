using IntegrationTests.Helpers;
using VIAPadelClub.Core.QueryContracts.Queries;
using VIAPadelClub.Infrastructure.EfcQueries.Queries;
using Xunit;
using Xunit.Abstractions;

namespace IntegrationTests.Queries;

public class PlayerPageOverviewQueryHandlerTest(ITestOutputHelper output)
{
    
    [Fact]
    public async Task HandleAsync_ValidPlayerEmail_ReturnsProfileData()
    {
        // Arrange
        var context = MyDbContext.CreateSeededContext();
        var handler = new PlayerPageOverviewQueryHandler(context);
        var query = new PlayerPageOverview.Query("123456@via.dk");

        // Act

        var result = await handler.HandleAsync(query);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Data);

        var data = result.Data;

        Assert.False(string.IsNullOrWhiteSpace(data.FirstName));
        Assert.False(string.IsNullOrWhiteSpace(data.LastName));
        Assert.Equal("123456@via.dk", data.Email);
        output.WriteLine($"Name: {data.FirstName} {data.LastName}, Email: {data.Email}");

        Assert.NotNull(data.ProfilePictureUrl);
        Assert.True(data.FutureBookingCount >= 0);
        Assert.All(data.UpcomingBookings, booking =>
        {
            output.WriteLine($"Upcoming: {booking.Date} at {booking.StartTime}");
            Assert.False(string.IsNullOrWhiteSpace(booking.Date));
        });

        Assert.All(data.PastBookings, booking =>
        {
            output.WriteLine($"Past: {booking.Date}");
            Assert.False(string.IsNullOrWhiteSpace(booking.Date));
        });
    }
    
}