using Domain.Enums;

namespace Domain.Entities;

public class Account
{
    public Guid Id { get; set; }

    public string Number { get; set; } = string.Empty;
    public Currency Currency { get; set; }
    public decimal Balance { get; set; }
    public AccountStatus Status { get; set; } = AccountStatus.Active;

    public Guid ClientId { get; set; }
    public Client Client { get; set; } = null!;

    public ICollection<Card> Cards { get; set; } = new List<Card>();
}
