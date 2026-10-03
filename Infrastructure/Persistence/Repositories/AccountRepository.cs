using Application.Abstractions.Persistence;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

internal sealed class AccountRepository(AppDbContext context) : IAccountRepository
{
    public Task<Account?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        context.Accounts
            .Include(account => account.Cards)
            .SingleOrDefaultAsync(account => account.Id == id, cancellationToken);

    public async Task<IReadOnlyList<Account>> ListByClientAsync(Guid clientId,
        CancellationToken cancellationToken = default) =>
        await context.Accounts
            .AsNoTracking()
            .Where(account => account.ClientId == clientId)
            .OrderBy(account => account.Number)
            .ToListAsync(cancellationToken);

    public Task<bool> ExistsForClientAsync(Guid clientId, CancellationToken cancellationToken = default) =>
        context.Accounts.AnyAsync(account => account.ClientId == clientId, cancellationToken);

    public void Add(Account account) => context.Accounts.Add(account);
}
