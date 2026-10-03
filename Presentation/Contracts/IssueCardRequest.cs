using Domain.Enums;

namespace Presentation.Contracts;

/// <summary>
/// Request body for issuing a card. The number and expiration date are set by the bank.
/// <see cref="Type"/> is nullable so a missing value is rejected instead of defaulting to the first card type.
/// </summary>
public sealed record IssueCardRequest(CardType? Type);
