using Domain.Entities;

namespace Application.Clients;

public sealed record ClientResponse(
    Guid Id,
    string LastName,
    string FirstName,
    string? MiddleName,
    string Email,
    DateOnly DateOfBirth)
{
    public static ClientResponse From(Client client) =>
        new(client.Id, client.LastName, client.FirstName, client.MiddleName, client.Email, client.DateOfBirth);
}
