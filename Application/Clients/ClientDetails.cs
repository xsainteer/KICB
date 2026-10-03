namespace Application.Clients;

/// <summary>The client data that can be set when creating or updating a client.</summary>
public sealed record ClientDetails(
    string LastName,
    string FirstName,
    string? MiddleName,
    string Email,
    DateOnly DateOfBirth);
