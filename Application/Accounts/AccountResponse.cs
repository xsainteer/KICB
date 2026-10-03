using Domain.Entities;
using Domain.Enums;

namespace Application.Accounts;

public sealed record AccountResponse(
    Guid Id,
    Guid ClientId,
    string Number,
    Currency Currency,
    decimal Balance,
    AccountStatus Status)
{
    public static AccountResponse From(Account account) =>
        new(account.Id, account.ClientId, account.Number, account.Currency, account.Balance, account.Status);
}
