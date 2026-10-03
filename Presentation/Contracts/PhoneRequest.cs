using Domain.Enums;

namespace Presentation.Contracts;

/// <summary>
/// Request body for adding and updating a client's phone.
/// <see cref="Type"/> is nullable so a missing value is rejected instead of defaulting to the first phone type.
/// </summary>
public sealed record PhoneRequest(string Number, PhoneType? Type);
