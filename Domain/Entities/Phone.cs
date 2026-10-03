using Domain.Enums;

namespace Domain.Entities;

public class Phone
{
    private Phone()
    {
    } // EF Core

    public Phone(Guid clientId, string number, PhoneType type)
    {
        Id = Guid.NewGuid();
        ClientId = clientId;
        Update(number, type);
    }

    public Guid Id { get; private set; }

    public string Number { get; private set; } = null!;
    public PhoneType Type { get; private set; }

    public Guid ClientId { get; private set; }
    public Client Client { get; private set; } = null!;

    public void Update(string number, PhoneType type)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(number);

        Number = number;
        Type = type;
    }
}
