using System.Net;
using IntegrationTests.Helpers;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using VIAPadelClub.Core.Domain.Aggregates.DailySchedules;
using VIAPadelClub.Core.Domain.Aggregates.DailySchedules.Contracts;
using VIAPadelClub.Core.Domain.Aggregates.DailySchedules.Values;
using VIAPadelClub.Core.Tools.OperationResult;
using VIAPadelClub.Infrastructure.EfcDmPersistence;
using VIAPadelClub.Infrastructure.EfcQueries.GeneratedModels;
using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;
using DailySchedule = VIAPadelClub.Core.Domain.Aggregates.DailySchedules.DailySchedule;

namespace IntegrationTests.WebApi.Command;

public class UpdateDailyScheduleDateAndTimeEndpointTest(ITestOutputHelper testOutputHelper)
{
    private readonly ITestOutputHelper _testOutputHelper = testOutputHelper;

    [Fact]
    public async Task UpdateScheduleDateAndTime_ShouldUpdateScheduleAndReturnNoContent()
    {
        // Arrange
        await using WebApplicationFactory<Program> webApplicationFactory = new PadelClubWebApplicationFactory();
        using var client = webApplicationFactory.CreateClient();

        var serviceScope = webApplicationFactory.Services.CreateScope();

        var veaDatabaseProductionContext = serviceScope.ServiceProvider.GetService<VeadatabaseProductionContext>()!;
        var domainModelContext = serviceScope.ServiceProvider.GetService<DomainModelContext>()!;
        var dateProvider = serviceScope.ServiceProvider.GetService<IDateProvider>()!;
        var scheduleRepository = serviceScope.ServiceProvider.GetService<IDailyScheduleRepository>()!;

        var scheduleId = ScheduleId.Create();

        var createScheduleResult = DailySchedule.CreateSchedule(dateProvider, scheduleId);

        await scheduleRepository.AddAsync(createScheduleResult.Data);
        await domainModelContext.SaveChangesAsync();

        var newDate = DateTime.Today.AddDays(3).ToString("yyyy-MM-dd");
        var newStartTime = TimeOnly.Parse("08:00").ToString("HH:mm:ss");
        var newEndTime = TimeOnly.Parse("09:00").ToString("HH:mm:ss");

        var updateScheduleDateTimeRequestBody = new
        {
            ScheduleId = scheduleId.Value,
            Date = newDate,
            StartTime = newStartTime,
            EndTime = newEndTime
        };

        // Act
        var response = await client.PutAsJsonAsync($"/api/dailySchedule/update", updateScheduleDateTimeRequestBody);

        // Assert
        var all = await veaDatabaseProductionContext.DailySchedules.ToListAsync();
        var updatedSchedule = all.FirstOrDefault(s =>
            string.Equals(s.ScheduleId, scheduleId.Value.ToString(), StringComparison.OrdinalIgnoreCase));

        Assert.True(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        Assert.NotNull(updatedSchedule);

        Assert.Equal(newDate, updatedSchedule.ScheduleDate);
    }

    [Fact]
    public async Task UpdateScheduleDateAndTime_ShouldReturnBadRequest_WhenEndTimeIsBeforeStartTime()
    {
        // Arrange
        await using WebApplicationFactory<Program> webApplicationFactory = new PadelClubWebApplicationFactory();
        using var client = webApplicationFactory.CreateClient();

        var serviceScope = webApplicationFactory.Services.CreateScope();

        var domainModelContext = serviceScope.ServiceProvider.GetRequiredService<DomainModelContext>();
        var dateProvider = serviceScope.ServiceProvider.GetRequiredService<IDateProvider>();
        var scheduleRepository = serviceScope.ServiceProvider.GetRequiredService<IDailyScheduleRepository>();

        var scheduleId = ScheduleId.Create();
        var createScheduleResult = DailySchedule.CreateSchedule(dateProvider, scheduleId);

        await scheduleRepository.AddAsync(createScheduleResult.Data);
        await domainModelContext.SaveChangesAsync();

        // Set invalid time range: EndTime is before StartTime
        var invalidStartTime = TimeOnly.Parse("10:00").ToString("HH:mm:ss");
        var invalidEndTime = TimeOnly.Parse("09:00").ToString("HH:mm:ss");

        var updateScheduleDateTimeRequestBody = new
        {
            ScheduleId = scheduleId.Value,
            Date = DateTime.Today.AddDays(3).ToString("yyyy-MM-dd"),
            StartTime = invalidStartTime,
            EndTime = invalidEndTime
        };

        // Act
        var response = await client.PutAsJsonAsync($"/api/dailySchedule/update", updateScheduleDateTimeRequestBody);

        // Assert
        Assert.False(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}