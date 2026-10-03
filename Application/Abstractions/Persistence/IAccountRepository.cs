using Domain.Entities;

namespace Application.Abstractions.Persistence;

/// <summary>
/// Repository for the <see cref="Account"/> aggregate. Accounts are closed, never deleted, so there is no <c>Remove</c>.
/// </summary>
public interface IAccountRepository
{
    /// <summary>
    /// Loads the whole aggregate — the account with its cards — so its rules (closing, blocking and unblocking
    /// cards) see the full state. Use this for every operation.
    /// </summary>
    Task<Account?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Read-only list for display: cards are <b>not</b> loaded and the accounts are not tracked,
    /// so they must not be used for operations.
    /// </summary>
    Task<IReadOnlyList<Account>> ListByClientAsync(Guid clientId, CancellationToken cancellationToken = default);

    Task<bool> ExistsForClientAsync(Guid clientId, CancellationToken cancellationToken = default);

    void Add(Account account);
}
