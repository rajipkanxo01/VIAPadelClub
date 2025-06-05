using IntegrationTests.Helpers;
using IntegrationTests.Seeders;
using Microsoft.AspNetCore.Mvc.Testing;
using VIAPadelClub.Infrastructure.EfcQueries.GeneratedModels;
using VIAPadelClub.Presentation.WebApi.Endpoints.Queries;
using Xunit;

namespace IntegrationTests.WebApi.Queries;

public class ViewManagerOverviewEndpointTest
{
    [Fact]
    public async Task ViewManagerOverview_ShouldReturnListOfDailyScheduleForMonth()
    {
        // Arrange
        string monthName = "April";

        await using WebApplicationFactory<Program> webApplicationFactory = new PadelClubWebApplicationFactory();
        using var client = webApplicationFactory.CreateClient();

        var serviceScope = webApplicationFactory.Services.CreateScope();

        var veaDatabaseProductionContext = serviceScope.ServiceProvider.GetService<VeadatabaseProductionContext>()!;
        veaDatabaseProductionContext.SeedTestData();

        // Act
        var response = await client.GetAsync($"/api/manager/overview?MonthName={monthName}");

        // Assert
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<ViewManagerOverviewResponse>();

        Assert.NotNull(result);
        Assert.NotEmpty(result!.Schedules);

        foreach (var schedule in result.Schedules)
        {
            Assert.False(string.IsNullOrWhiteSpace(schedule.Id));
            Assert.False(string.IsNullOrWhiteSpace(schedule.Date));
            Assert.False(string.IsNullOrWhiteSpace(schedule.Status));
            Assert.True(schedule.CourtCount >= 0);
        }
    }
    
    [Fact]
    public async Task ViewManagerOverview_ShouldReturnBadRequest_WhenMonthNameIsMissing()
    {
        // Arrange
        await using WebApplicationFactory<Program> webApplicationFactory = new PadelClubWebApplicationFactory();
        using var client = webApplicationFactory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/manager/overview");

        // Assert
        Assert.False(response.IsSuccessStatusCode);
        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);

        var errorMessage = await response.Content.ReadAsStringAsync();
        Assert.Contains("MonthName", errorMessage, StringComparison.OrdinalIgnoreCase);
    }

}