using Domain.Enums;

namespace Domain.Entities;

public class Phone
{
    public Guid Id { get; set; }

    public string Number { get; set; } = string.Empty;
    public PhoneType Type { get; set; }

    public Guid ClientId { get; set; }
    public Client Client { get; set; } = null!;
}
