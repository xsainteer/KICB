namespace Presentation.Contracts;

/// <summary>
/// Request body for deposit and withdraw: <c>{ "amount": 150.00 }</c>.
/// A record rather than a bare <c>decimal</c> so that <see cref="Validation.AmountRequestValidator"/> can target it
/// and fields can be added later without breaking clients. Shared because both bodies are identical today;
/// split it into separate requests if they diverge.
/// </summary>
public sealed record AmountRequest(decimal Amount);
