using Microsoft.EntityFrameworkCore;

namespace VIAPadelClub.Infrastructure.EfcDmPersistence;

public class DomainModelContext(DbContextOptions<DomainModelContext> options) : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(DomainModelContext).Assembly);
    }
    
}