using Domain.Entities;
using Domain.Enums;

namespace Application.Cards;

/// <summary>
/// A card as exposed by the API. The full card number is never returned, only its last four digits.
/// </summary>
public sealed record CardResponse(
    Guid Id,
    Guid AccountId,
    string MaskedNumber,
    DateOnly ExpirationDate,
    CardType Type,
    CardStatus Status)
{
    private const int VisibleDigits = 4;

    public static CardResponse From(Card card) =>
        new(card.Id, card.AccountId, Mask(card.Number), card.ExpirationDate, card.Type, card.Status);

    private static string Mask(string number) =>
        number.Length <= VisibleDigits
            ? number
            : new string('*', number.Length - VisibleDigits) + number[^VisibleDigits..];
}
