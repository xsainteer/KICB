namespace Domain.Exceptions;

public sealed class AccountInactiveException : DomainException
{
    public AccountInactiveException(Guid accountId)
        : base($"Account {accountId} is closed and cannot be operated on.")
    {
    }
}
