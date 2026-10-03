using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

internal sealed class PhoneConfiguration : IEntityTypeConfiguration<Phone>
{
    public void Configure(EntityTypeBuilder<Phone> builder)
    {
        builder.HasKey(phone => phone.Id);
        builder.Property(phone => phone.Id).ValueGeneratedNever();

        builder.Property(phone => phone.Number).HasMaxLength(20).IsRequired();
        builder.Property(phone => phone.Type).HasConversion<string>().HasMaxLength(10);
    }
}
