﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VIAPadelClub.Core.Domain.Aggregates.DailySchedules;
using VIAPadelClub.Core.Domain.Aggregates.DailySchedules.Entities;
using VIAPadelClub.Core.Domain.Aggregates.DailySchedules.Values;
using VIAPadelClub.Core.Domain.Aggregates.Players.Values;
using VIAPadelClub.Core.Domain.Common.BaseClasses;

namespace VIAPadelClub.Infrastructure.EfcDmPersistence.Configs;

public class BookingEntityConfig : IEntityTypeConfiguration<Booking>
{
    public void Configure(EntityTypeBuilder<Booking> builder)
    {
        builder.HasKey(x => x.BookingId);
        builder
            .Property(m => m.BookingId)
            .IsRequired()
            .HasConversion(
                mId => mId.Value,
                dbValue => BookingId.FromGuid(dbValue)
            );
        
        builder.Property(b => b.BookedBy)
            .HasConversion(
                email => email.Value,
                dbValue => Email.Create(dbValue).Data
            )
            .HasColumnName("BookedBy");

        builder.Property(b => b.Duration).HasColumnName("Duration");
        builder.Property(b => b.StartTime).HasColumnName("StartTime");
        builder.Property(b => b.EndTime).HasColumnName("EndTime");
        builder.Property(b => b.BookedDate).HasColumnName("BookedDate");

        builder.Property(b => b.BookingStatus)
            .HasConversion(
                status => status.ToString(),
                str => Enum.Parse<BookingStatus>(str)
            )
            .HasColumnName("BookingStatus");

        builder.HasOne<Core.Domain.Aggregates.Players.Player>()
            .WithMany()
            .HasForeignKey(booking => booking.BookedBy);
        
        builder.HasOne(b => b.Court)
            .WithMany()
            .HasForeignKey("Name", "ScheduleId")
            .HasPrincipalKey("Name", "ScheduleId")
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.ToTable("Bookings");
    }
}