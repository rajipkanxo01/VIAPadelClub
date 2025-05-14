using System.Text.Json;
using IntegrationTests.Helpers;
using VIAPadelClub.Core.QueryContracts.Queries;
using VIAPadelClub.Infrastructure.EfcQueries.Queries;
using Xunit;
using Xunit.Abstractions;

namespace IntegrationTests.Queries;

public class ViewManagerOverviewQueryHandlerTest(ITestOutputHelper output)
{
    private readonly ITestOutputHelper _output = output;

    [Fact]
    public async Task HandleAsync_ValidMonth_ReturnsSchedule()
    {
        // Arrange
        var context = MyDbContext.CreateSeededContext();
        var handler = new ViewManagerOverviewHandler(context);

        var query = new ViewManagerOverview.Query("April");

        // Act
        var result = await handler.HandleAsync(query);
        
        // Print Output
        foreach (var schedule in result.Data.Schedules)
        {
            _output.WriteLine("Schedule:");
            _output.WriteLine($"  ID: {schedule.Id}");
            _output.WriteLine($"  Date: {schedule.Date}");
            _output.WriteLine($"  Status: {schedule.Status}");
            _output.WriteLine($"  Court Count: {schedule.CourtCount}");
            _output.WriteLine("---------------------------");
        }


        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.All(result.Data.Schedules, schedule =>
        {
            Assert.False(string.IsNullOrWhiteSpace(schedule.Id));
            Assert.False(string.IsNullOrWhiteSpace(schedule.Date));
            Assert.False(string.IsNullOrWhiteSpace(schedule.Status));
            Assert.True(schedule.CourtCount > 0);
            
            var parsedDate = DateTime.Parse(schedule.Date);
            Assert.Equal(parsedDate.Month, 4); // April
        });
    }
    
    [Fact]
    public async Task HandleAsync_InvalidMonth_ReturnsFailure()
    {
        // Arrange
        var context = MyDbContext.CreateSeededContext();
        var handler = new ViewManagerOverviewHandler(context);
        var query = new ViewManagerOverview.Query("Invalid Date");

        // Act
        var result = await handler.HandleAsync(query);

        // Assert
        Assert.False(result.Success);
        Assert.Contains("Invalid month name", result.ErrorMessage);
    }
}