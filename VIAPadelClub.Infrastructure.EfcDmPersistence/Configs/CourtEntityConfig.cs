using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VIAPadelClub.Core.Domain.Aggregates.DailySchedules.Entities;
using VIAPadelClub.Core.Domain.Aggregates.DailySchedules.Values;

namespace VIAPadelClub.Infrastructure.EfcDmPersistence.Configs;

public class CourtEntityConfig : IEntityTypeConfiguration<Court>
{
    public void Configure(EntityTypeBuilder<Court> builder)
    {
        builder.HasKey(c => c.Name);

        builder.Property(c => c.Name)
            .ValueGeneratedNever();

        builder.Property(c => c.Name)
            .HasConversion(
                name => name.Value,
                value => CourtName.Create(value).Data
            )
            .HasColumnName("CourtName");

        builder.ToTable("Courts");
    }
    
}