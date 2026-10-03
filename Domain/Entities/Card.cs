using Domain.Common;
using Domain.Enums;
using Domain.Errors;

namespace Domain.Entities;

public class Card
{
    private Card()
    {
    } // EF Core

    /// <summary>Cards are issued through <see cref="Entities.Account.IssueCard"/>.</summary>
    internal Card(Guid accountId, string number, DateOnly expirationDate, CardType type)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(number);

        Id = Guid.NewGuid();
        AccountId = accountId;
        Number = number;
        ExpirationDate = expirationDate;
        Type = type;
        Status = CardStatus.Active;
    }

    public Guid Id { get; private set; }

    public string Number { get; private set; } = null!;
    public DateOnly ExpirationDate { get; private set; }
    public CardType Type { get; private set; }
    public CardStatus Status { get; private set; }

    public Guid AccountId { get; private set; }
    public Account Account { get; private set; } = null!;

    public Result Block()
    {
        if (Status == CardStatus.Blocked)
            return CardErrors.AlreadyBlocked(Id);

        Status = CardStatus.Blocked;
        return Result.Success();
    }

    /// <remarks>
    /// Requires <see cref="Account"/> to be loaded: a closed account must not regain an active card.
    /// </remarks>
    public Result Unblock()
    {
        if (Status == CardStatus.Active)
            return CardErrors.NotBlocked(Id);
        if (Account.IsClosed)
            return AccountErrors.Inactive(AccountId);

        Status = CardStatus.Active;
        return Result.Success();
    }
}
