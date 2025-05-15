using System.Globalization;
using Microsoft.EntityFrameworkCore;
using VIAPadelClub.Core.QueryContracts.Queries;
using VIAPadelClub.Infrastructure.EfcQueries.Queries;
using Xunit;
using Xunit.Abstractions;
using IntegrationTests.Helpers;

namespace IntegrationTests.Queries;

public class PlayerScheduleOverviewQueryHandlerTest(ITestOutputHelper output)
{
    [Fact]
    public async Task HandleAsync_ValidMonth_ReturnsSchedulesWithCorrectStatus()
    {
        // Arrange
        var context = MyDbContext.CreateSeededContext();
        var handler = new PlayerScheduleOverviewQueryHandler(context);
        var query = new PlayerScheduleOverview.Query("June");

        // Act
        var result = await handler.HandleAsync(query);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.All(result.Data.Schedules, schedule =>
        {
            output.WriteLine($"ID: {schedule.Id}, Date: {schedule.Date}, Status: {schedule.Status}");

            Assert.False(string.IsNullOrWhiteSpace(schedule.Id));
            Assert.False(string.IsNullOrWhiteSpace(schedule.Date));
            Assert.False(string.IsNullOrWhiteSpace(schedule.Status));

            DateTime.TryParse(schedule.Date, out var parsedDate);
            Assert.Equal(6, parsedDate.Month); // June
        });
    }
    
    [Fact]
    public async Task HandleAsync_InvalidMonth_ReturnsFailure()
    {
        // Arrange
        var context = MyDbContext.CreateSeededContext();
        var handler = new PlayerScheduleOverviewQueryHandler(context);
        var query = new PlayerScheduleOverview.Query("NotAMonth");

        // Act
        var result = await handler.HandleAsync(query);

        // Assert
        Assert.False(result.Success);
        Assert.Contains("Invalid month name", result.ErrorMessage);
    }
}