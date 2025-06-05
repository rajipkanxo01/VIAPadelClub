using System.Net;
using IntegrationTests.Helpers;
using Microsoft.AspNetCore.Mvc.Testing;
using VIAPadelClub.Core.Domain.Aggregates.DailySchedules.Contracts;
using Xunit;

namespace IntegrationTests.WebApi.Command;

public class CreateDailyScheduleEndpointTest
{
    [Fact]
    public async Task CreateDailySchedule_ShouldReturnOk()
    {
        // Arrange
        await using WebApplicationFactory<Program> webApplicationFactory = new PadelClubWebApplicationFactory();
        using var client = webApplicationFactory.CreateClient();

        // Act
        var response = await client.PostAsync("/api/dailySchedule/create", JsonContent.Create(new { }));

        // Assert
        Assert.True(response.IsSuccessStatusCode);
        Assert.True(response.StatusCode == HttpStatusCode.NoContent);
    }
}