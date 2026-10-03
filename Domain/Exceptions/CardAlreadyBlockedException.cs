namespace Domain.Exceptions;

public sealed class CardAlreadyBlockedException : DomainException
{
    public CardAlreadyBlockedException(Guid cardId)
        : base($"Card {cardId} is already blocked.")
    {
    }
}
