using Domain.Common;

namespace Application.Common;

/// <summary>
/// Lookups that found nothing. Loading data is an application concern — the domain never does it — so these
/// errors live here. <c>Card.NotFound</c> stays in the domain: the Account aggregate looks its cards up itself.
/// </summary>
public static class NotFoundErrors
{
    public static Error Client(Guid clientId) =>
        new("Client.NotFound",
            $"Client {clientId} was not found.",
            ErrorType.NotFound);

    public static Error Phone(Guid phoneId) =>
        new("Phone.NotFound",
            $"Phone {phoneId} was not found for this client.",
            ErrorType.NotFound);

    public static Error Account(Guid accountId) =>
        new("Account.NotFound",
            $"Account {accountId} was not found.",
            ErrorType.NotFound);
}
