namespace Application.Common;

/// <summary>
/// The data was changed by another request after it was loaded. Thrown by <see cref="Abstractions.Persistence.IUnitOfWork"/>
/// so that Application does not depend on EF Core's own exception type.
/// </summary>
public sealed class ConcurrencyConflictException : Exception
{
    public ConcurrencyConflictException(Exception innerException)
        : base("The data was modified by another request. Reload it and try again.", innerException)
    {
    }
}
