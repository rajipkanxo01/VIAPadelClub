using System.Net;
using IntegrationTests.Helpers;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using VIAPadelClub.Core.Domain.Aggregates.DailySchedules;
using VIAPadelClub.Core.Domain.Aggregates.DailySchedules.Contracts;
using VIAPadelClub.Core.Domain.Aggregates.DailySchedules.Entities;
using VIAPadelClub.Core.Domain.Aggregates.DailySchedules.Values;
using VIAPadelClub.Core.Domain.Common;
using VIAPadelClub.Infrastructure.EfcDmPersistence;
using Xunit;
using Xunit.Abstractions;

namespace IntegrationTests.WebApi.Command;

public class CreateBookingEndpointTest(ITestOutputHelper testOutputHelper)
{
    private readonly ITestOutputHelper _testOutputHelper = testOutputHelper;

    [Fact]
    public async Task CreateBooking_ShouldSucceed_WhenInputIsValid()
    {
        // Arrange
        await using var factory = new PadelClubWebApplicationFactory();
        using var client = factory.CreateClient();

        using var scope = factory.Services.CreateScope();
        var services = scope.ServiceProvider;

        var dateProvider = services.GetRequiredService<IDateProvider>();
        var scheduleFinder = services.GetRequiredService<IScheduleFinder>();
        var scheduleRepo = services.GetRequiredService<IDailyScheduleRepository>();
        var dbContext = services.GetRequiredService<DomainModelContext>();

        var scheduleId = ScheduleId.Create();
        var createResult = DailySchedule.CreateSchedule(dateProvider, scheduleId);
        var schedule = createResult.Data;

        await scheduleRepo.AddAsync(schedule);
        await dbContext.SaveChangesAsync();

        var courtName = CourtName.Create("S1").Data;
        var court = Court.Create(courtName).Data;

        var addCourtResult = await schedule.AddAvailableCourt(court, dateProvider, scheduleFinder);
        Assert.True(addCourtResult.Success, $"Court could not be added: {addCourtResult.ErrorMessage}");

        await dbContext.SaveChangesAsync();

        var activationResult = schedule.Activate(dateProvider);
        Assert.True(activationResult.Success, $"Schedule could not be activated: {activationResult.ErrorMessage}");

        await dbContext.SaveChangesAsync();

        _testOutputHelper.WriteLine("✅ Schedule created, court added, and activated.");

        var requestBody = new
        {
            dailyScheduleId = scheduleId.Value.ToString().ToUpper(),
            bookedBy = "test@via.dk",
            startTime = "09:00",
            endTime = "12:00",
            courtName = "S1"
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/schedules/createBooking", requestBody);

        var responseBody = await response.Content.ReadAsStringAsync();
        _testOutputHelper.WriteLine($"Response status: {(int)response.StatusCode}");
        _testOutputHelper.WriteLine($"Response body: {responseBody}");

        // Assert
        Assert.True(response.IsSuccessStatusCode, "Expected success status code.");
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }
}
