using Domain.Common;

namespace Application.Clients;

/// <summary>
/// Client rules that need the database (unique email, existing accounts), so they are checked by
/// <see cref="ClientService"/> rather than by the <c>Client</c> entity.
/// </summary>
public static class ClientErrors
{
    public static Error EmailTaken(string email) =>
        new("Client.EmailTaken",
            $"Email {email} is already used by another client.",
            ErrorType.Conflict);

    public static Error HasAccounts(Guid clientId) =>
        new("Client.HasAccounts",
            $"Client {clientId} cannot be deleted: they have accounts.",
            ErrorType.Conflict);
}
