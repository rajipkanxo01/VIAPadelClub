using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VIAPadelClub.Core.Domain.Aggregates.DailySchedules;
using VIAPadelClub.Core.Domain.Aggregates.DailySchedules.Values;
using VIAPadelClub.Core.Domain.Aggregates.Players;
using VIAPadelClub.Core.Domain.Aggregates.Players.Entities;
using VIAPadelClub.Core.Domain.Aggregates.Players.Values;
using VIAPadelClub.Infrastructure.EfcDmPersistence.Configs;

namespace VIAPadelClub.Infrastructure.EfcDmPersistence;

public class DomainModelContext(DbContextOptions options) : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(DomainModelContext).Assembly);
    }
    
}