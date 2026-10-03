using Application.Clients;

namespace Presentation.Contracts;

/// <summary>Request body for creating and updating a client: both take the full set of client details.</summary>
public sealed record ClientRequest(
    string LastName,
    string FirstName,
    string? MiddleName,
    string Email,
    DateOnly DateOfBirth)
{
    public ClientDetails ToDetails() => new(LastName, FirstName, MiddleName, Email, DateOfBirth);
}
