using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VIAPadelClub.Core.Domain.Aggregates.DailySchedules;
using VIAPadelClub.Core.Domain.Aggregates.DailySchedules.Values;
using VIAPadelClub.Core.Domain.Aggregates.Players;
using VIAPadelClub.Core.Domain.Aggregates.Players.Entities;
using VIAPadelClub.Core.Domain.Aggregates.Players.Values;

namespace VIAPadelClub.Infrastructure.EfcDmPersistence;

public class DomainModelContext(DbContextOptions options) : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(DomainModelContext).Assembly);
        ConfigurePlayer(modelBuilder.Entity<Player>());
        ConfigureDailySchedule(modelBuilder.Entity<DailySchedule>());
    }

    private static void ConfigureDailySchedule(EntityTypeBuilder<DailySchedule> entityBuilder)
    {
        entityBuilder.HasKey(schedule => schedule.scheduleId);

        entityBuilder
            .Property(m => m.scheduleId)
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
        

    }

    private static void ConfigurePlayer(EntityTypeBuilder<Player> entityBuilder)
    {
        entityBuilder.HasKey(player => player.email);

        entityBuilder
            .Property(p => p.email)
            .IsRequired()
            .HasConversion(
                eId => eId.Value,
                dbValue => Email.Create(dbValue).Data
            );

        entityBuilder.ComplexProperty<FullName>(p => p.fullName, propBuilder =>
        {
            propBuilder.Property(valueObject => valueObject.FirstName)
                .HasColumnName("FirstName");
            propBuilder.Property(valueObject => valueObject.LastName)
                .HasColumnName("LastName");
        });

        entityBuilder.ComplexProperty<ProfileUri>(p => p.url,
            builder => { builder.Property(value => value.Value).HasColumnName("ProfileUrl"); });

        entityBuilder.ComplexProperty<VipMemberShip>(p => p.vipMemberShip, builder =>
        {
            builder.Property<DateOnly>(ship => ship.StartDate).HasColumnName("StartDate");
            builder.Property<DateOnly>(ship => ship.EndDate).HasColumnName("EndDate");
            builder.Property<bool>(ship => ship.IsVIP).HasColumnName("IsVip");
        });

        entityBuilder.OwnsOne(p => p.activeQuarantine, quarantineBuilder =>
        {
            quarantineBuilder.Property(q => q.StartDate).HasColumnName("QuarantineStartDate");
            quarantineBuilder.Property(q => q.EndDate).HasColumnName("QuarantineEndDate");
        });

        entityBuilder.Property(p => p.isBlackListed).IsRequired();
        entityBuilder.Property(p => p.isQuarantined).IsRequired();
    }
}