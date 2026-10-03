using Application.Abstractions.Persistence;
using Application.Common;
using Domain.Common;
using Domain.Entities;
using Domain.Errors;

namespace Application.Clients;

public sealed class ClientService(IClientRepository clients, IAccountRepository accounts, IUnitOfWork unitOfWork)
{
    public async Task<PagedResult<ClientResponse>> ListAsync(ClientFilter filter, int page, int pageSize,
        CancellationToken cancellationToken = default)
    {
        var clientsPage = await clients.ListAsync(filter, page, pageSize, cancellationToken);
        return clientsPage.Map(ClientResponse.From);
    }

    public async Task<Result<ClientResponse>> GetAsync(Guid clientId, CancellationToken cancellationToken = default)
    {
        var client = await clients.GetByIdAsync(clientId, cancellationToken);
        if (client is null)
            return ClientErrors.NotFound(clientId);

        return ClientResponse.From(client);
    }

    public async Task<Result<ClientResponse>> CreateAsync(ClientDetails details,
        CancellationToken cancellationToken = default)
    {
        if (await clients.IsEmailTakenAsync(details.Email, cancellationToken: cancellationToken))
            return ClientErrors.EmailTaken(details.Email);

        var client = new Client(
            details.LastName, details.FirstName, details.MiddleName, details.Email, details.DateOfBirth);
        clients.Add(client);

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return ClientResponse.From(client);
    }

    public async Task<Result<ClientResponse>> UpdateAsync(Guid clientId, ClientDetails details,
        CancellationToken cancellationToken = default)
    {
        var client = await clients.GetByIdAsync(clientId, cancellationToken);
        if (client is null)
            return ClientErrors.NotFound(clientId);
        if (await clients.IsEmailTakenAsync(details.Email, exceptClientId: clientId, cancellationToken))
            return ClientErrors.EmailTaken(details.Email);

        client.Update(details.LastName, details.FirstName, details.MiddleName, details.Email, details.DateOfBirth);

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return ClientResponse.From(client);
    }

    /// <summary>
    /// Deletes a client together with their phones. A client who has accounts cannot be deleted:
    /// accounts hold money and history, so they are closed, never deleted.
    /// </summary>
    public async Task<Result> DeleteAsync(Guid clientId, CancellationToken cancellationToken = default)
    {
        var client = await clients.GetByIdAsync(clientId, cancellationToken);
        if (client is null)
            return ClientErrors.NotFound(clientId);
        if (await accounts.ExistsForClientAsync(clientId, cancellationToken))
            return ClientErrors.HasAccounts(clientId);

        clients.Remove(client);

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
