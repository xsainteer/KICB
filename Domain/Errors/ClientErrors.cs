using Domain.Common;

namespace Domain.Errors;

public static class ClientErrors
{
    public static Error NotFound(Guid clientId) =>
        new("Client.NotFound",
            $"Client {clientId} was not found.",
            ErrorType.NotFound);

    public static Error EmailTaken(string email) =>
        new("Client.EmailTaken",
            $"Email {email} is already used by another client.",
            ErrorType.Conflict);

    public static Error HasAccounts(Guid clientId) =>
        new("Client.HasAccounts",
            $"Client {clientId} cannot be deleted: they have accounts.",
            ErrorType.Conflict);
}
