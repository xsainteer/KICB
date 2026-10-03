using System.Security.Cryptography;
using Application.Abstractions;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Numbers;

/// <summary>
/// Generates random account and card numbers and retries until one is unused.
/// The unique indexes on both columns remain the final guard against a race between two requests.
/// </summary>
internal sealed class NumberGenerator(AppDbContext context) : INumberGenerator
{
    private const int AccountNumberLength = 16;
    private const int CardNumberLength = 16;

    /// <summary>Placeholder issuer prefix (BIN); a real bank uses the one assigned by its card network.</summary>
    private const string CardIssuerPrefix = "400000";

    private const int MaxAttempts = 10;

    public Task<string> NewAccountNumberAsync(CancellationToken cancellationToken = default) =>
        GenerateUniqueAsync(
            () => RandomDigits(AccountNumberLength),
            number => context.Accounts.AnyAsync(account => account.Number == number, cancellationToken));

    /// <summary>A 16-digit card number: issuer prefix, random digits and a Luhn check digit.</summary>
    public Task<string> NewCardNumberAsync(CancellationToken cancellationToken = default) =>
        GenerateUniqueAsync(
            () =>
            {
                var payload = CardIssuerPrefix + RandomDigits(CardNumberLength - CardIssuerPrefix.Length - 1);
                return payload + LuhnCheckDigit(payload);
            },
            number => context.Cards.AnyAsync(card => card.Number == number, cancellationToken));

    private static async Task<string> GenerateUniqueAsync(Func<string> generate, Func<string, Task<bool>> isTaken)
    {
        for (var attempt = 0; attempt < MaxAttempts; attempt++)
        {
            var number = generate();
            if (!await isTaken(number))
                return number;
        }

        throw new InvalidOperationException($"Could not generate an unused number in {MaxAttempts} attempts.");
    }

    private static string RandomDigits(int length) =>
        string.Create(length, 0, (digits, _) =>
        {
            for (var i = 0; i < digits.Length; i++)
                digits[i] = (char)('0' + RandomNumberGenerator.GetInt32(10));
        });

    /// <summary>
    /// Luhn check digit: walking right to left, every second digit starting from the rightmost one of the payload
    /// is doubled (minus 9 when above 9); the check digit brings the total to a multiple of 10.
    /// </summary>
    private static char LuhnCheckDigit(string payload)
    {
        var sum = 0;
        for (var i = payload.Length - 1; i >= 0; i--)
        {
            var digit = payload[i] - '0';
            if ((payload.Length - 1 - i) % 2 == 0)
            {
                digit *= 2;
                if (digit > 9)
                    digit -= 9;
            }

            sum += digit;
        }

        return (char)('0' + (10 - sum % 10) % 10);
    }
}
