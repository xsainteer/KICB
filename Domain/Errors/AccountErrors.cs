using Domain.Common;
using Domain.Enums;

namespace Domain.Errors;

public static class AccountErrors
{
    public static Error Inactive(Guid accountId) =>
        new("Account.Inactive",
            $"Account {accountId} is closed and cannot be operated on.",
            ErrorType.Conflict);

    public static Error InsufficientFunds(Guid accountId, decimal balance, decimal requestedAmount) =>
        new("Account.InsufficientFunds",
            $"Account {accountId} has insufficient funds: balance {balance}, requested {requestedAmount}.",
            ErrorType.Conflict);

    public static Error NotEmpty(Guid accountId, decimal balance) =>
        new("Account.NotEmpty",
            $"Account {accountId} cannot be closed: balance is {balance}, must be zero.",
            ErrorType.Conflict);

    public static Error HasActiveCards(Guid accountId) =>
        new("Account.HasActiveCards",
            $"Account {accountId} cannot be closed: it still has active cards.",
            ErrorType.Conflict);

    public static Error SameAccount(Guid accountId) =>
        new("Account.SameAccount",
            $"Cannot transfer from account {accountId} to itself.",
            ErrorType.Validation);

    public static Error CurrencyMismatch(Currency source, Currency destination) =>
        new("Account.CurrencyMismatch",
            $"Cannot transfer between accounts with different currencies: {source} -> {destination}.",
            ErrorType.Conflict);
}
