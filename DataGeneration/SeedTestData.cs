using System.Text.Json;
using VIAPadelClub.Infrastructure.EfcDmPersistence;

public static class TestDataSeeder
{
    public static void SeedTestData(this DomainModelContext context)
    {
        // Seed Players
        var playersJson = File.ReadAllText(@"c:\Users\Work-B\Downloads\Players.json");
        var players = JsonSerializer.Deserialize<List<Player>>(playersJson);
        context.Players.AddRange(players);

        // Seed Daily Schedules
        var dailySchedulesJson = File.ReadAllText(@"c:\Users\Work-B\Downloads\DailySchedules.json");
        var dailySchedules = JsonSerializer.Deserialize<List<DailySchedule>>(dailySchedulesJson);
        context.DailySchedules.AddRange(dailySchedules);

        // Seed Courts
        var courtsJson = File.ReadAllText(@"c:\Users\Work-B\Downloads\Court.json");
        var courts = JsonSerializer.Deserialize<List<Court>>(courtsJson);
        context.Courts.AddRange(courts);

        // Seed VIP Intervals
        var vipIntervalsJson = File.ReadAllText(@"c:\Users\Work-B\Downloads\VipIntervals.json");
        var vipIntervals = JsonSerializer.Deserialize<List<VipInterval>>(vipIntervalsJson);
        context.VipIntervals.AddRange(vipIntervals);

        // Seed Court Bookings
        var courtBookingsJson = File.ReadAllText(@"c:\Users\Work-B\Downloads\CourtBooking.json");
        var courtBookings = JsonSerializer.Deserialize<List<CourtBooking>>(courtBookingsJson);
        context.CourtBookings.AddRange(courtBookings);

        // Save changes to the database
        context.SaveChanges();
    }
}