using Domain.Enums;

namespace Domain.Entities;

public class Card
{
    public Guid Id { get; set; }

    public string Number { get; set; } = string.Empty;
    public DateOnly ExpirationDate { get; set; }
    public CardType Type { get; set; }
    public CardStatus Status { get; set; } = CardStatus.Active;

    public Guid AccountId { get; set; }
    public Account Account { get; set; } = null!;
}
