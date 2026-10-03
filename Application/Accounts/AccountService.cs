using Application.Abstractions.Persistence;
using Domain.Common;
using Domain.Entities;
using Domain.Errors;

namespace Application.Accounts;

/// <summary>
/// Business operations on a single account and its cards. The rules live in the <see cref="Account"/> aggregate;
/// this service loads it, runs the operation and saves the result.
/// </summary>
public sealed class AccountService(IAccountRepository accounts, IUnitOfWork unitOfWork)
{
    public Task<Result<AccountResponse>> DepositAsync(Guid accountId, decimal amount,
        CancellationToken cancellationToken = default) =>
        ExecuteAsync(accountId, account => account.Deposit(amount), AccountResponse.From, cancellationToken);

    public Task<Result<AccountResponse>> WithdrawAsync(Guid accountId, decimal amount,
        CancellationToken cancellationToken = default) =>
        ExecuteAsync(accountId, account => account.Withdraw(amount), AccountResponse.From, cancellationToken);

    public Task<Result<AccountResponse>> CloseAsync(Guid accountId, CancellationToken cancellationToken = default) =>
        ExecuteAsync(accountId, account => account.Close(), AccountResponse.From, cancellationToken);

    public Task<Result<CardResponse>> BlockCardAsync(Guid accountId, Guid cardId,
        CancellationToken cancellationToken = default) =>
        ExecuteAsync(accountId, account => account.BlockCard(cardId), account => CardOf(account, cardId),
            cancellationToken);

    public Task<Result<CardResponse>> UnblockCardAsync(Guid accountId, Guid cardId,
        CancellationToken cancellationToken = default) =>
        ExecuteAsync(accountId, account => account.UnblockCard(cardId), account => CardOf(account, cardId),
            cancellationToken);

    private async Task<Result<TResponse>> ExecuteAsync<TResponse>(
        Guid accountId,
        Func<Account, Result> operation,
        Func<Account, TResponse> toResponse,
        CancellationToken cancellationToken)
    {
        var account = await accounts.GetByIdAsync(accountId, cancellationToken);
        if (account is null)
            return AccountErrors.NotFound(accountId);

        var result = operation(account);
        if (result.IsFailure)
            return result.Error!;

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return toResponse(account);
    }

    private static CardResponse CardOf(Account account, Guid cardId) =>
        CardResponse.From(account.Cards.Single(card => card.Id == cardId));
}
