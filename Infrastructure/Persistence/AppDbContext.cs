using Domain.Entities;
using Infrastructure.Persistence.Configurations;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Client> Clients => Set<Client>();
    public DbSet<Phone> Phones => Set<Phone>();
    public DbSet<Account> Accounts => Set<Account>();
    public DbSet<Card> Cards => Set<Card>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresExtension("citext");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }

    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        BumpAccountVersions();
        return base.SaveChanges(acceptAllChangesOnSuccess);
    }

    public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess,
        CancellationToken cancellationToken = default)
    {
        BumpAccountVersions();
        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    /// <summary>
    /// Account is the aggregate root for its cards: a card change alone does not touch the account row,
    /// so the version is bumped explicitly to make concurrent changes to the same aggregate conflict.
    /// </summary>
    private void BumpAccountVersions()
    {
        var changedAccountIds = ChangeTracker.Entries<Card>()
            .Where(entry => entry.State is EntityState.Added or EntityState.Modified)
            .Select(entry => entry.Entity.AccountId)
            .Concat(ChangeTracker.Entries<Account>()
                .Where(entry => entry.State == EntityState.Modified)
                .Select(entry => entry.Entity.Id))
            .ToHashSet();

        foreach (var entry in ChangeTracker.Entries<Account>())
        {
            if (entry.State != EntityState.Added && changedAccountIds.Contains(entry.Entity.Id))
                entry.Property<int>(AccountConfiguration.VersionProperty).CurrentValue++;
        }
    }
}
