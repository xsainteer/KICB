using Domain.Common;

namespace Domain.Errors;

public static class CardErrors
{
    public static Error NotFound(Guid cardId) =>
        new("Card.NotFound",
            $"Card {cardId} was not found on this account.",
            ErrorType.NotFound);

    public static Error AlreadyBlocked(Guid cardId) =>
        new("Card.AlreadyBlocked",
            $"Card {cardId} is already blocked.",
            ErrorType.Conflict);

    public static Error NotBlocked(Guid cardId) =>
        new("Card.NotBlocked",
            $"Card {cardId} is not blocked.",
            ErrorType.Conflict);
}
