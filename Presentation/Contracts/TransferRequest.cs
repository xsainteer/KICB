namespace Presentation.Contracts;

public sealed record TransferRequest(Guid SourceAccountId, Guid DestinationAccountId, decimal Amount);
