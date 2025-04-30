using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VIAPadelClub.Core.Domain.Aggregates.Players.Entities;
using VIAPadelClub.Core.Domain.Aggregates.Players.Values;

namespace VIAPadelClub.Infrastructure.EfcDmPersistence.Configs.Player;

public class PlayerEntityConfig : IEntityTypeConfiguration<Core.Domain.Aggregates.Players.Player>
{
    public void Configure(EntityTypeBuilder<Core.Domain.Aggregates.Players.Player> entityBuilder)
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

        entityBuilder.ToTable("Player");
    }
}