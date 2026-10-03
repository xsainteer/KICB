using Application.Abstractions.Persistence;

namespace Presentation.Contracts;

/// <summary>
/// Query string for listing clients, e.g. <c>?lastName=ivan&amp;bornFrom=1990-01-01&amp;page=2&amp;pageSize=20</c>.
/// A class with settable properties rather than a record so that omitted paging parameters keep their defaults.
/// </summary>
public sealed class ClientListQuery
{
    public string? LastName { get; init; }
    public string? FirstName { get; init; }
    public string? Email { get; init; }
    public DateOnly? BornFrom { get; init; }
    public DateOnly? BornTo { get; init; }

    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;

    public ClientFilter ToFilter() => new(LastName, FirstName, Email, BornFrom, BornTo);
}
