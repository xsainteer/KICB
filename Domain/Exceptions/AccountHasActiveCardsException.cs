namespace Domain.Exceptions;

public sealed class AccountHasActiveCardsException : DomainException
{
    public AccountHasActiveCardsException(Guid accountId)
        : base($"Account {accountId} cannot be closed: it still has active cards.")
    {
    }
}
