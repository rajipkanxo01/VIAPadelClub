using Microsoft.EntityFrameworkCore;

namespace IntegrationTests.Seeders;

using Helpers;
using VIAPadelClub.Infrastructure.EfcQueries.GeneratedModels;
using Xunit;

public class TestDatabaseSeederTest
{
    private static  VeadatabaseProductionContext CreateContext()
    {
        var path = Path.Combine(Path.GetTempPath(), $"test_{Guid.NewGuid()}.db");
        var options = new DbContextOptionsBuilder<VeadatabaseProductionContext>()
            .UseSqlite($"Data Source={path}")
            .Options;

        var context = new VeadatabaseProductionContext(options);
         context.Database.EnsureCreated();

        context.SeedTestData();
        context.ChangeTracker.Clear();

        return context;
    }

    [Fact]
    public void ShouldSeedDatabaseWithTestData()
    {
        // Arrange & Act
        var context = CreateContext();

        // Assert
        Assert.NotEmpty(context.Players);
        Assert.NotEmpty(context.DailySchedules);
        Assert.NotEmpty(context.Courts);
        Assert.NotEmpty(context.VipTimeRanges);
        Assert.NotEmpty(context.Bookings);
    }
}