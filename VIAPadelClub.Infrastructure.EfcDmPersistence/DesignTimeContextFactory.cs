using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace VIAPadelClub.Infrastructure.EfcDmPersistence;

public class DesignTimeContextFactory : IDesignTimeDbContextFactory<DomainModelContext>
{
    public DomainModelContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<DomainModelContext>();
        optionsBuilder.UseSqlite(@"Data Source = VEADatabaseProduction.db");
        return new DomainModelContext(optionsBuilder.Options);
    }
}