using Microsoft.EntityFrameworkCore;
using VIAPadelClub.Core.Domain.Aggregates.DailySchedules;
using VIAPadelClub.Core.Domain.Aggregates.DailySchedules.Entities;
using VIAPadelClub.Core.Domain.Aggregates.Players;
using VIAPadelClub.Infrastructure.EfcDmPersistence;

namespace IntegrationTests.Helpers;

public class MyDbContext(DbContextOptions options) : DomainModelContext(options)
{
    public DbSet<DailySchedule> DailySchedules { get; set; }
    // public DbSet<Court> Courts { get; set; }
    // public DbSet<Booking> Bookings { get; set; }
    public DbSet<Player> Players { get; set; }

    public static MyDbContext SetupContext()
    {
        DbContextOptionsBuilder<MyDbContext> optionsBuilder = new();
        string currentDir = Directory.GetCurrentDirectory();
        string testDbName = Path.Combine(currentDir, "Test" + Guid.NewGuid() + ".db");
        optionsBuilder.UseSqlite(@"Data Source = " + testDbName);
        MyDbContext context = new(optionsBuilder.Options);

        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();
        return context;
    }

    public static async Task SaveAndClearAsync<T>(T entity, MyDbContext context)
        where T : class
    {
        await context.Set<T>().AddAsync(entity);
        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();
    }
}