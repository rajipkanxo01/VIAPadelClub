using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VIAPadelClub.Core.Domain.Aggregates.DailySchedules;
using VIAPadelClub.Core.Domain.Aggregates.DailySchedules.Values;
using VIAPadelClub.Core.Domain.Aggregates.Players.Values;

namespace VIAPadelClub.Infrastructure.EfcDmPersistence.Configs;

public class DailyScheduleEntityConfig
{
    public static void ConfigureDailySchedule(EntityTypeBuilder<DailySchedule> entityBuilder)
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

        entityBuilder.OwnsMany(
            schedule => schedule.listOfCourts,
            ownedBuilder =>
            {
                ownedBuilder.WithOwner().HasForeignKey("DailyScheduleId");

                ownedBuilder
                    .Property(p => p.Name)
                    .HasConversion(
                        v => v.Value,
                        v => CourtName.Create(v).Data 
                    )
                    .HasColumnName("Name");

                ownedBuilder.ToTable("Courts");
                ownedBuilder.HasKey("DailyScheduleId", "Name"); // composite key
            });
        
        entityBuilder.OwnsMany(
            schedule => schedule.listOfAvailableCourts,
            ownedBuilder =>
            {
                ownedBuilder.WithOwner().HasForeignKey("DailyScheduleId");

                ownedBuilder
                    .Property(p => p.Name)
                    .HasConversion(
                        v => v.Value,
                        v => CourtName.Create(v).Data 
                    )
                    .HasColumnName("Name");

                ownedBuilder.ToTable("AvailableCourts");
                ownedBuilder.HasKey("DailyScheduleId", "Name"); // composite key
            });

        entityBuilder.OwnsMany(
        schedule => schedule.listOfBookings,
        ownedBuilder => 
        {
            ownedBuilder.WithOwner().HasForeignKey("DailyScheduleId");

            ownedBuilder.Property(p => p.BookingId)
                .HasColumnName("BookingId")
                .IsRequired();

            ownedBuilder.Property(p => p.BookedBy)
                .HasConversion(
                email => email.Value,
                dbValue => Email.Create(dbValue).Data
                )
                .HasColumnName("BookedBy");
            
            ownedBuilder.OwnsOne(
            p => p.Court,
            ownedBuilder =>
            {
                ownedBuilder.Property(c => c.Name)
                    .HasConversion(
                    name => name.Value,
                    value => CourtName.Create(value).Data
                    )
                    .HasColumnName("CourtName");
            });
            
            ownedBuilder.Property(p => p.Duration).HasColumnName("Duration");
            ownedBuilder.Property(p => p.StartTime).HasColumnName("StartTime");
            ownedBuilder.Property(p => p.EndTime).HasColumnName("EndTime");
            ownedBuilder.Property(p => p.BookedDate).HasColumnName("BookedDate");

            ownedBuilder.Property(p => p.BookingStatus)
                .HasConversion(
                status => status.ToString(),
                str => Enum.Parse<BookingStatus>(str)
                )
                .HasColumnName("BookingStatus");
            
            ownedBuilder.ToTable("Bookings");
            ownedBuilder.HasKey("DailyScheduleId", "BookingId");
            
            ownedBuilder.Ignore(p => p.Id);
        });
    }
}