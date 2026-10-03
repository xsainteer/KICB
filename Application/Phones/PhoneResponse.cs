using Domain.Entities;
using Domain.Enums;

namespace Application.Phones;

public sealed record PhoneResponse(Guid Id, Guid ClientId, string Number, PhoneType Type)
{
    public static PhoneResponse From(Phone phone) => new(phone.Id, phone.ClientId, phone.Number, phone.Type);
}
