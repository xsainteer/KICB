using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

internal sealed class AccountConfiguration : IEntityTypeConfiguration<Account>
{
    /// <summary>
    /// Shadow concurrency token for the whole Account aggregate, bumped by <see cref="AppDbContext"/>.
    /// It is a persistence concern, so it lives only in the EF model, not on the entity.
    /// </summary>
    public const string VersionProperty = "Version";

    public void Configure(EntityTypeBuilder<Account> builder)
    {
        builder.HasKey(account => account.Id);
        builder.Property(account => account.Id).ValueGeneratedNever();

        builder.Property(account => account.Number).HasMaxLength(34).IsRequired();
        builder.HasIndex(account => account.Number).IsUnique();

        builder.Property(account => account.Currency).HasConversion<string>().HasMaxLength(3);
        builder.Property(account => account.Balance).HasPrecision(18, 2);
        builder.Property(account => account.Status).HasConversion<string>().HasMaxLength(10);

        builder.Property<int>(VersionProperty).IsConcurrencyToken();

        builder.HasMany(account => account.Cards)
            .WithOne(card => card.Account)
            .HasForeignKey(card => card.AccountId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
