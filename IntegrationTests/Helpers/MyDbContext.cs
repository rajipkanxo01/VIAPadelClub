using Microsoft.EntityFrameworkCore;
using Moq;
using UnitTests.Features.Helpers;
using VIAPadelClub.Core.Domain.Aggregates.DailySchedules;
using VIAPadelClub.Core.Domain.Aggregates.DailySchedules.Contracts;
using VIAPadelClub.Core.Domain.Aggregates.DailySchedules.Values;
using VIAPadelClub.Infrastructure.EfcDmPersistence.Configs;
using Xunit;
using Assert = Xunit.Assert;

namespace IntegrationTests.Helpers;

public class MyDbContext(DbContextOptions options) : Microsoft.EntityFrameworkCore.DbContext(options)
{
    public DbSet<DailySchedule> DailySchedules { get; set; }

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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        DailyScheduleEntityConfig.ConfigureDailySchedule(modelBuilder.Entity<DailySchedule>());
    }
}