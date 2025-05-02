using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VIAPadelClub.Core.Domain.Aggregates.DailySchedules;
using VIAPadelClub.Core.Domain.Aggregates.DailySchedules.Entities;
using VIAPadelClub.Core.Domain.Aggregates.DailySchedules.Values;
using VIAPadelClub.Core.Domain.Aggregates.Players.Values;

namespace VIAPadelClub.Infrastructure.EfcDmPersistence.Configs;

public class DailyScheduleEntityConfig : IEntityTypeConfiguration<DailySchedule>
{
    public void Configure(EntityTypeBuilder<DailySchedule> entityBuilder)
    {
        entityBuilder.HasKey(schedule => schedule.ScheduleId);

        entityBuilder
            .Property(m => m.ScheduleId)
            .IsRequired()
            .HasConversion(
                mId => mId.Value,
                dbValue => ScheduleId.FromGuid(dbValue)
            );

        entityBuilder.Property<ScheduleStatus>(name => name.status)
            .HasConversion(
                status => status.ToString(),
                value => (ScheduleStatus)Enum.Parse(typeof(ScheduleStatus), value)
            );

        entityBuilder.Property(schedule => schedule.availableFrom).IsRequired();
        entityBuilder.Property(schedule => schedule.availableUntil).IsRequired();
        entityBuilder.Property(schedule => schedule.scheduleDate).IsRequired();
        entityBuilder.Property(schedule => schedule.isDeleted).IsRequired();

        entityBuilder.OwnsMany(
            schedule => schedule.vipTimeRanges,
            ownedBuilder =>
            {
                ownedBuilder.WithOwner().HasForeignKey("DailyScheduleId");
                ownedBuilder.Property(p => p.Start).HasColumnName("VipStart");
                ownedBuilder.Property(p => p.End).HasColumnName("VipEnd");
                ownedBuilder.ToTable("VipTimeRanges");
                ownedBuilder.HasKey("DailyScheduleId", "Start", "End"); // composite key
            });

        /*entityBuilder
            .HasMany(ds => ds.listOfCourts)
            .WithMany(c => c.Schedules)
            .UsingEntity<Dictionary<string, object>>(
                "DailyScheduleCourts",
                j => j
                    .HasOne<Court>()
                    .WithMany()
                    .HasForeignKey("CourtName")
                    .HasConstraintName("FK_DailyScheduleCourts_Courts_CourtName")
                    .OnDelete(DeleteBehavior.Cascade),
                j => j
                    .HasOne<DailySchedule>()
                    .WithMany()
                    .HasForeignKey("ScheduleId")
                    .HasConstraintName("FK_DailyScheduleCourts_DailySchedules_ScheduleId")
                    .OnDelete(DeleteBehavior.Cascade),
                j =>
                {
                    j.HasKey("ScheduleId", "CourtName");
                    j.ToTable("DailyScheduleCourts");
                }
            );*/
        
        entityBuilder
            .HasMany<Booking>(ds => ds.listOfBookings)
            .WithOne() 
            .HasForeignKey("ScheduleId")
            .OnDelete(DeleteBehavior.Cascade);

        entityBuilder
            .HasMany<Court>(ds => ds.listOfCourts)
            .WithOne()
            .HasForeignKey(court => court.ScheduleId)
            .OnDelete(DeleteBehavior.Cascade);

        entityBuilder.ToTable("DailySchedules");
    }
}