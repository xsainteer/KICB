using Application.Abstractions;
using Application.Abstractions.Persistence;
using Application.Common;
using Domain.Common;
using Domain.Entities;
using Domain.Enums;
using Domain.Errors;

namespace Application.Cards;

/// <summary>
/// Issuing, reading, blocking and unblocking cards. Cards belong to the <see cref="Account"/> aggregate,
/// so every operation loads the account and goes through it.
/// </summary>
public sealed class CardService(
    IAccountRepository accounts,
    INumberGenerator numberGenerator,
    TimeProvider timeProvider,
    IUnitOfWork unitOfWork)
{
    private const int ValidityYears = 3;

    public async Task<Result<IReadOnlyList<CardResponse>>> ListAsync(Guid accountId,
        CancellationToken cancellationToken = default)
    {
        var account = await accounts.GetByIdAsync(accountId, cancellationToken);
        if (account is null)
            return NotFoundErrors.Account(accountId);

        return Result<IReadOnlyList<CardResponse>>.Success(account.Cards.Select(CardResponse.From).ToList());
    }

    public async Task<Result<CardResponse>> GetAsync(Guid accountId, Guid cardId,
        CancellationToken cancellationToken = default)
    {
        var account = await accounts.GetByIdAsync(accountId, cancellationToken);
        if (account is null)
            return NotFoundErrors.Account(accountId);

        var card = FindCard(account, cardId);
        if (card is null)
            return CardErrors.NotFound(cardId);

        return CardResponse.From(card);
    }

    /// <summary>Issues a card valid until the end of the month, <see cref="ValidityYears"/> years from today.</summary>
    public async Task<Result<CardResponse>> IssueAsync(Guid accountId, CardType type,
        CancellationToken cancellationToken = default)
    {
        var account = await accounts.GetByIdAsync(accountId, cancellationToken);
        if (account is null)
            return NotFoundErrors.Account(accountId);

        var number = await numberGenerator.NewCardNumberAsync(cancellationToken);
        var issued = account.IssueCard(number, ExpirationDate(), type);
        if (issued.IsFailure)
            return issued.Error!;

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return CardResponse.From(issued.Value);
    }

    public Task<Result<CardResponse>> BlockAsync(Guid accountId, Guid cardId,
        CancellationToken cancellationToken = default) =>
        ExecuteAsync(accountId, cardId, account => account.BlockCard(cardId), cancellationToken);

    public Task<Result<CardResponse>> UnblockAsync(Guid accountId, Guid cardId,
        CancellationToken cancellationToken = default) =>
        ExecuteAsync(accountId, cardId, account => account.UnblockCard(cardId), cancellationToken);

    private async Task<Result<CardResponse>> ExecuteAsync(
        Guid accountId,
        Guid cardId,
        Func<Account, Result> operation,
        CancellationToken cancellationToken)
    {
        var account = await accounts.GetByIdAsync(accountId, cancellationToken);
        if (account is null)
            return NotFoundErrors.Account(accountId);

        var result = operation(account);
        if (result.IsFailure)
            return result.Error!;

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return CardResponse.From(FindCard(account, cardId)!);
    }

    private DateOnly ExpirationDate()
    {
        var expiresIn = DateOnly.FromDateTime(timeProvider.GetUtcNow().UtcDateTime).AddYears(ValidityYears);
        return new DateOnly(expiresIn.Year, expiresIn.Month, DateTime.DaysInMonth(expiresIn.Year, expiresIn.Month));
    }

    private static Card? FindCard(Account account, Guid cardId) =>
        account.Cards.SingleOrDefault(card => card.Id == cardId);
}
