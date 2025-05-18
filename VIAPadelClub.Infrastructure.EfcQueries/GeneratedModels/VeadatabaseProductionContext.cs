using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace VIAPadelClub.Infrastructure.EfcQueries.GeneratedModels;

public partial class VeadatabaseProductionContext : DbContext
{
    public VeadatabaseProductionContext()
    {
    }

    public VeadatabaseProductionContext(DbContextOptions<VeadatabaseProductionContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Booking> Bookings { get; set; }

    public virtual DbSet<Court> Courts { get; set; }

    public virtual DbSet<DailySchedule> DailySchedules { get; set; }

    public virtual DbSet<EfmigrationsLock> EfmigrationsLocks { get; set; }

    public virtual DbSet<Player> Players { get; set; }

    public virtual DbSet<VipTimeRange> VipTimeRanges { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlite("Data Source=VEADatabaseProduction.db");
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Booking>(entity =>
        {
            entity.HasIndex(e => e.BookedBy, "IX_Bookings_BookedBy");

            entity.HasIndex(e => new { e.Name, e.ScheduleId }, "IX_Bookings_Name_ScheduleId");

            entity.HasIndex(e => e.ScheduleId, "IX_Bookings_ScheduleId");

            entity.HasOne(d => d.BookedByNavigation).WithMany(p => p.Bookings).HasForeignKey(d => d.BookedBy);

            entity.HasOne(d => d.Schedule).WithMany(p => p.Bookings).HasForeignKey(d => d.ScheduleId);

            entity.HasOne(d => d.Court).WithMany(p => p.Bookings)
                .HasForeignKey(d => new { d.Name, d.ScheduleId })
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Court>(entity =>
        {
            entity.HasKey(e => new { e.CourtName, e.ScheduleId });

            entity.HasIndex(e => e.ScheduleId, "IX_Courts_ScheduleId");

            entity.HasOne(d => d.Schedule).WithMany(p => p.Courts).HasForeignKey(d => d.ScheduleId);
        });

        modelBuilder.Entity<DailySchedule>(entity =>
        {
            entity.HasKey(e => e.ScheduleId);

            entity.Property(e => e.AvailableFrom).HasColumnName("availableFrom");
            entity.Property(e => e.AvailableUntil).HasColumnName("availableUntil");
            entity.Property(e => e.IsDeleted).HasColumnName("isDeleted");
            entity.Property(e => e.ScheduleDate).HasColumnName("scheduleDate");
            entity.Property(e => e.Status).HasColumnName("status");
        });

        modelBuilder.Entity<EfmigrationsLock>(entity =>
        {
            entity.ToTable("__EFMigrationsLock");

            entity.Property(e => e.Id).ValueGeneratedNever();
        });

        modelBuilder.Entity<Player>(entity =>
        {
            entity.HasKey(e => e.Email);

            entity.ToTable("Player");

            entity.Property(e => e.Email).HasColumnName("email");
            entity.Property(e => e.IsBlackListed).HasColumnName("isBlackListed");
            entity.Property(e => e.IsQuarantined).HasColumnName("isQuarantined");
            entity.Property(e => e.VipendDate).HasColumnName("VIPEndDate");
            entity.Property(e => e.VipstartDate).HasColumnName("VIPStartDate");
        });

        modelBuilder.Entity<VipTimeRange>(entity =>
        {
            entity.HasKey(e => new { e.DailyScheduleId, e.VipStart, e.VipEnd });

            entity.HasOne(d => d.DailySchedule).WithMany(p => p.VipTimeRanges).HasForeignKey(d => d.DailyScheduleId);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
