namespace IntegrationTests.Seeders;

using Helpers;
using VIAPadelClub.Infrastructure.EfcQueries.GeneratedModels;
using Xunit;

public class TestDatabaseSeederTest
{
    private static VeadatabaseProductionContext CreateContext()
    {
        VeadatabaseProductionContext context = new();
        context.Database.EnsureDeleted();
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