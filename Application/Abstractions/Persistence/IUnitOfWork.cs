using Application.Common;

namespace Application.Abstractions.Persistence;

public interface IUnitOfWork
{
    /// <summary>Saves all changes made through the repositories in one transaction.</summary>
    /// <exception cref="ConcurrencyConflictException">The data was changed by another request after it was loaded.</exception>
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
