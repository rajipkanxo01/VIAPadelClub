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
            .HasConversion(
                name => name.Value,
                value => CourtName.Create(value).Data
            )
            .HasColumnName("CourtName");
        
        /*builder.Property(c => c.DailyScheduleId)
            .HasConversion(
                id => id.Value,
                value => ScheduleId.FromGuid(value)
            )
            .HasColumnName("ScheduleId");*/


        builder.ToTable("Courts");
    }
    
}