using System.Net;
using IntegrationTests.Helpers;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Services.Contracts;
using VIAPadelClub.Core.Domain.Aggregates.DailySchedules;
using VIAPadelClub.Core.Domain.Aggregates.DailySchedules.Contracts;
using VIAPadelClub.Core.Domain.Aggregates.DailySchedules.Values;
using VIAPadelClub.Infrastructure.EfcDmPersistence;
using VIAPadelClub.Infrastructure.EfcQueries.GeneratedModels;
using Xunit;
using Xunit.Abstractions;
using DailySchedule = VIAPadelClub.Core.Domain.Aggregates.DailySchedules.DailySchedule;

public class AddCourtToDailyScheduleEndpointTest(ITestOutputHelper testOutputHelper)
{
    private readonly ITestOutputHelper _testOutputHelper = testOutputHelper;

    [Fact]
    public async Task AddCourtToSchedule_ShouldSucceed_WhenInputIsValid()
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
        
        // 👇 This is critical
        var scheduleFinder = serviceScope.ServiceProvider.GetRequiredService<IScheduleFinder>() as ScheduleFinder;
        scheduleFinder?.AddSchedule(createScheduleResult.Data);
        
        veaDatabaseProductionContext.DailySchedules.Add(
            new VIAPadelClub.Infrastructure.EfcQueries.GeneratedModels.DailySchedule
            {
                ScheduleId = scheduleId.Value.ToString().ToLowerInvariant(),
                AvailableFrom = "15:00:00",
                AvailableUntil = "22:00:00",
                IsDeleted = 0,
                ScheduleDate = DateTime.Today.ToString("yyyy-MM-dd"),
                Status = "Draft"
            });

        await veaDatabaseProductionContext.SaveChangesAsync();

        var requestBody = new
        {
            RequestBody = new
            {
                ScheduleId = scheduleId.Value.ToString().ToLowerInvariant(),
                CourtName = "SS1"
            }
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/schedules/addCourt", requestBody);

        var content = await response.Content.ReadAsStringAsync();
        _testOutputHelper.WriteLine($"Status: {(int)response.StatusCode}, Body: {content}");
        var count = await veaDatabaseProductionContext.DailySchedules.CountAsync();

        _testOutputHelper.WriteLine($"Schedules in read DB: {count}");
        _testOutputHelper.WriteLine($"Inserted ScheduleId: {scheduleId.Value}");
        var allSchedules = await veaDatabaseProductionContext.DailySchedules.ToListAsync();
        foreach (var s in allSchedules)
        {
            _testOutputHelper.WriteLine($"Read DB ScheduleId: {s.ScheduleId}");
        }

        // Assert


        var updatedSchedule = await veaDatabaseProductionContext.DailySchedules
            .Include(s => s.Courts)
            .FirstOrDefaultAsync(s => s.ScheduleId == scheduleId.Value.ToString());
        Assert.True(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        Assert.NotNull(updatedSchedule);
        Assert.Contains(updatedSchedule.Courts, c => c.CourtName == "SS1");
    }
}