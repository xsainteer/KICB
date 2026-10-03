using Domain.Common;
using Domain.Enums;
using Domain.Errors;

namespace Domain.Entities;

/// <summary>
/// Part of the <see cref="Entities.Account"/> aggregate: cards are issued, blocked and unblocked through their account.
/// </summary>
public class Card
{
    private Card()
    {
    } // EF Core

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

    internal Result Block()
    {
        if (Status == CardStatus.Blocked)
            return CardErrors.AlreadyBlocked(Id);

        Status = CardStatus.Blocked;
        return Result.Success();
    }

    internal Result Unblock()
    {
        if (Status == CardStatus.Active)
            return CardErrors.NotBlocked(Id);

        Status = CardStatus.Active;
        return Result.Success();
    }
}
