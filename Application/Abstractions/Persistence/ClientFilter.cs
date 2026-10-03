namespace Application.Abstractions.Persistence;

/// <summary>
/// Criteria for listing clients. Text fields match case-insensitively by substring; null fields are ignored.
/// </summary>
public sealed record ClientFilter(
    string? LastName = null,
    string? FirstName = null,
    string? Email = null,
    DateOnly? BornFrom = null,
    DateOnly? BornTo = null);
