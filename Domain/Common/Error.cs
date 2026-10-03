namespace Domain.Common;

/// <summary>
/// An expected business failure. Unlike exceptions, errors are part of the normal control flow.
/// </summary>
/// <param name="Code">Stable machine-readable identifier, e.g. "Account.InsufficientFunds".</param>
/// <param name="Message">Human-readable description.</param>
/// <param name="Type">Failure category, used by the API layer to pick a status code.</param>
public sealed record Error(string Code, string Message, ErrorType Type);
