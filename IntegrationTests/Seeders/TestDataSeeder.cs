namespace IntegrationTests.Seeders;

using System.Text.Json;
using VIAPadelClub.Infrastructure.EfcQueries.GeneratedModels;

public static class TestDataSeeder
{
    public static void SeedTestData(this VeadatabaseProductionContext context)
    {
        // Seed Players
        var playersJson = File.ReadAllText(@"Seeders\data\Players.json");
        var players = JsonSerializer.Deserialize<List<Player>>(playersJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        context.Players.AddRange(players.AsEnumerable());

        // Seed Daily Schedules
        var dailySchedulesJson = File.ReadAllText(@"Seeders\data\DailySchedules.json");
        var dailySchedules = JsonSerializer.Deserialize<List<DailySchedule>>(dailySchedulesJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        context.DailySchedules.AddRange(dailySchedules.AsEnumerable());

        // Seed Courts
        var courtsJson = File.ReadAllText(@"Seeders\data\Court.json");
        var courts = JsonSerializer.Deserialize<List<Court>>(courtsJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        context.Courts.AddRange(courts.AsEnumerable());

        // Seed VIP Intervals
        var vipIntervalsJson = File.ReadAllText(@"Seeders\data\VipTimeRange.json");
        var vipIntervals = JsonSerializer.Deserialize<List<VipTimeRange>>(vipIntervalsJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        context.VipTimeRanges.AddRange(vipIntervals.AsEnumerable());

        // Seed Court Bookings
        var bookingsJson = File.ReadAllText(@"Seeders\data\Booking.json");
        var bookings = JsonSerializer.Deserialize<List<Booking>>(bookingsJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        context.Bookings.AddRange(bookings);

        // Save changes to the database
        context.SaveChanges();
    }
}