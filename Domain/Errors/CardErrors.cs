using Domain.Common;

namespace Domain.Errors;

public static class CardErrors
{
    public static Error AlreadyBlocked(Guid cardId) =>
        new("Card.AlreadyBlocked",
            $"Card {cardId} is already blocked.",
            ErrorType.Conflict);

    public static Error NotBlocked(Guid cardId) =>
        new("Card.NotBlocked",
            $"Card {cardId} is not blocked.",
            ErrorType.Conflict);
}
