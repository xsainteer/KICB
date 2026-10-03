using Domain.Entities;

namespace Application.Abstractions.Persistence;

/// <summary>
/// Repository for the <see cref="Account"/> aggregate. Accounts are always loaded together with their cards,
/// so the aggregate's rules (closing, blocking and unblocking cards) see its full state.
/// Accounts are closed, never deleted, so there is no <c>Remove</c>.
/// </summary>
public interface IAccountRepository
{
    Task<Account?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Account>> ListByClientAsync(Guid clientId, CancellationToken cancellationToken = default);

    Task<bool> ExistsForClientAsync(Guid clientId, CancellationToken cancellationToken = default);

    void Add(Account account);
}
