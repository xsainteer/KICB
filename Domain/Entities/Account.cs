using Domain.Common;
using Domain.Enums;
using Domain.Errors;

namespace Domain.Entities;

/// <summary>
/// Aggregate root for an account and its cards: the rule "a closed account has no active cards" spans both,
/// so cards are issued, blocked and unblocked only through their account.
/// </summary>
public class Account
{
    private readonly List<Card> _cards = new();

    private Account()
    {
    } // EF Core

    public Account(Guid clientId, string number, Currency currency)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(number);

        Id = Guid.NewGuid();
        ClientId = clientId;
        Number = number;
        Currency = currency;
        Status = AccountStatus.Active;
    }

    public Guid Id { get; private set; }

    public string Number { get; private set; } = null!;
    public Currency Currency { get; private set; }
    public decimal Balance { get; private set; }
    public AccountStatus Status { get; private set; }

    public Guid ClientId { get; private set; }
    public Client Client { get; private set; } = null!;

    public IReadOnlyCollection<Card> Cards => _cards.AsReadOnly();

    public bool IsClosed => Status == AccountStatus.Closed;

    public Result Deposit(decimal amount)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(amount);

        if (IsClosed)
            return AccountErrors.Inactive(Id);

        Balance += amount;
        return Result.Success();
    }

    public Result Withdraw(decimal amount)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(amount);

        if (IsClosed)
            return AccountErrors.Inactive(Id);
        if (Balance < amount)
            return AccountErrors.InsufficientFunds(Id, Balance, amount);

        Balance -= amount;
        return Result.Success();
    }

    /// <summary>
    /// Moves funds to another account. Both accounts are validated before either balance changes,
    /// so a failed transfer leaves both untouched.
    /// </summary>
    public Result TransferTo(Account destination, decimal amount)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(amount);

        if (destination.Id == Id)
            return AccountErrors.SameAccount(Id);
        if (IsClosed)
            return AccountErrors.Inactive(Id);
        if (destination.IsClosed)
            return AccountErrors.Inactive(destination.Id);
        if (destination.Currency != Currency)
            return AccountErrors.CurrencyMismatch(Currency, destination.Currency);
        if (Balance < amount)
            return AccountErrors.InsufficientFunds(Id, Balance, amount);

        Balance -= amount;
        destination.Balance += amount;
        return Result.Success();
    }

    public Result<Card> IssueCard(string number, DateOnly expirationDate, CardType type)
    {
        if (IsClosed)
            return AccountErrors.Inactive(Id);

        var card = new Card(Id, number, expirationDate, type);
        _cards.Add(card);
        return card;
    }

    /// <remarks>Requires <see cref="Cards"/> to be loaded.</remarks>
    public Result BlockCard(Guid cardId)
    {
        var card = FindCard(cardId);
        if (card is null)
            return CardErrors.NotFound(cardId);

        return card.Block();
    }

    /// <remarks>Requires <see cref="Cards"/> to be loaded.</remarks>
    public Result UnblockCard(Guid cardId)
    {
        var card = FindCard(cardId);
        if (card is null)
            return CardErrors.NotFound(cardId);
        if (IsClosed)
            return AccountErrors.Inactive(Id);

        return card.Unblock();
    }

    /// <remarks>Requires <see cref="Cards"/> to be loaded, otherwise the active-card check is skipped.</remarks>
    public Result Close()
    {
        if (IsClosed)
            return AccountErrors.Inactive(Id);
        if (Balance != 0)
            return AccountErrors.NotEmpty(Id, Balance);
        if (_cards.Any(card => card.Status == CardStatus.Active))
            return AccountErrors.HasActiveCards(Id);

        Status = AccountStatus.Closed;
        return Result.Success();
    }

    private Card? FindCard(Guid cardId) => _cards.SingleOrDefault(card => card.Id == cardId);
}
