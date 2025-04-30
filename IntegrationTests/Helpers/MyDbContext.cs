using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using VIAPadelClub.Infrastructure.EfcDmPersistence;

namespace IntegrationTests.Helpers;

public class MyDbContext(DbContextOptions options) : DomainModelContext(options)
{
    public static MyDbContext SetupContext()
    {
        var connection = new SqliteConnection("DataSource=:memory:");
        // var connection = new SqliteConnection("DataSource=TestDatabase.db");
        connection.Open();

        var options = new DbContextOptionsBuilder<DomainModelContext>()
            .UseSqlite(connection)
            .Options;

        var context = new MyDbContext(options);
        context.Database.EnsureCreated();

        return context;
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(DomainModelContext).Assembly);
    }

    public static async Task SaveAndClearAsync<T>(T entity, MyDbContext context)
        where T : class
    {
        await context.Set<T>().AddAsync(entity);
        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();
    }
    
    public static async Task ClearAllDataAsync(DomainModelContext context)
    {
        var tables = new[]
        {
            "DailyScheduleCourts",
            "VipTimeRanges",
            "Bookings",
            "Player",
            "Courts",
            "DailySchedules"
        };

        foreach (var table in tables)
        {
            await context.Database.ExecuteSqlRawAsync($"DELETE FROM \"{table}\"");
        }

        context.ChangeTracker.Clear();
    }


}