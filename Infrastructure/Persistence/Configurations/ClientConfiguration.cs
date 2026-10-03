using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

internal sealed class ClientConfiguration : IEntityTypeConfiguration<Client>
{
    public void Configure(EntityTypeBuilder<Client> builder)
    {
        builder.HasKey(client => client.Id);
        builder.Property(client => client.Id).ValueGeneratedNever();

        builder.Property(client => client.LastName).HasMaxLength(100).IsRequired();
        builder.Property(client => client.FirstName).HasMaxLength(100).IsRequired();
        builder.Property(client => client.MiddleName).HasMaxLength(100);

        // citext makes the unique index and lookups case-insensitive.
        builder.Property(client => client.Email).HasColumnType("citext").HasMaxLength(254).IsRequired();
        builder.HasIndex(client => client.Email).IsUnique();

        builder.HasIndex(client => new { client.LastName, client.FirstName });

        builder.HasMany(client => client.Phones)
            .WithOne(phone => phone.Client)
            .HasForeignKey(phone => phone.ClientId)
            .OnDelete(DeleteBehavior.Cascade);

        // A client who has accounts cannot be deleted: accounts hold money and are closed, never deleted.
        builder.HasMany(client => client.Accounts)
            .WithOne(account => account.Client)
            .HasForeignKey(account => account.ClientId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
