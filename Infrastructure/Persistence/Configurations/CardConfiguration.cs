using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

internal sealed class CardConfiguration : IEntityTypeConfiguration<Card>
{
    public void Configure(EntityTypeBuilder<Card> builder)
    {
        builder.HasKey(card => card.Id);
        builder.Property(card => card.Id).ValueGeneratedNever();

        builder.Property(card => card.Number).HasMaxLength(19).IsRequired();
        builder.HasIndex(card => card.Number).IsUnique();

        builder.Property(card => card.Type).HasConversion<string>().HasMaxLength(10);
        builder.Property(card => card.Status).HasConversion<string>().HasMaxLength(10);
    }
}
