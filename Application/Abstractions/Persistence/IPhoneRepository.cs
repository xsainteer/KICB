using Domain.Entities;

namespace Application.Abstractions.Persistence;

public interface IPhoneRepository
{
    Task<Phone?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Phone>> ListByClientAsync(Guid clientId, CancellationToken cancellationToken = default);

    void Add(Phone phone);

    void Remove(Phone phone);
}
