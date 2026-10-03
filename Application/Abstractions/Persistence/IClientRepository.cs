using Application.Common;
using Domain.Entities;

namespace Application.Abstractions.Persistence;

public interface IClientRepository
{
    Task<Client?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<PagedResult<Client>> ListAsync(ClientFilter filter, int page, int pageSize,
        CancellationToken cancellationToken = default);

    Task<bool> IsEmailTakenAsync(string email, Guid? exceptClientId = null,
        CancellationToken cancellationToken = default);

    void Add(Client client);

    void Remove(Client client);
}
