using Application.Abstractions;
using Application.Abstractions.Persistence;
using Domain.Common;
using Domain.Entities;
using Domain.Enums;
using Domain.Errors;

namespace Application.Accounts;

/// <summary>
/// Opening, reading and operating on accounts. The rules live in the <see cref="Account"/> aggregate;
/// this service loads it, runs the operation and saves the result.
/// </summary>
public sealed class AccountService(
    IAccountRepository accounts,
    IClientRepository clients,
    INumberGenerator numberGenerator,
    IUnitOfWork unitOfWork)
{
    public async Task<Result<IReadOnlyList<AccountResponse>>> ListByClientAsync(Guid clientId,
        CancellationToken cancellationToken = default)
    {
        if (!await clients.ExistsAsync(clientId, cancellationToken))
            return ClientErrors.NotFound(clientId);

        var clientAccounts = await accounts.ListByClientAsync(clientId, cancellationToken);
        return Result<IReadOnlyList<AccountResponse>>.Success(clientAccounts.Select(AccountResponse.From).ToList());
    }

    public async Task<Result<AccountResponse>> GetAsync(Guid accountId, CancellationToken cancellationToken = default)
    {
        var account = await accounts.GetByIdAsync(accountId, cancellationToken);
        if (account is null)
            return AccountErrors.NotFound(accountId);

        return AccountResponse.From(account);
    }

    public async Task<Result<AccountResponse>> OpenAsync(Guid clientId, Currency currency,
        CancellationToken cancellationToken = default)
    {
        if (!await clients.ExistsAsync(clientId, cancellationToken))
            return ClientErrors.NotFound(clientId);

        var number = await numberGenerator.NewAccountNumberAsync(cancellationToken);
        var account = new Account(clientId, number, currency);
        accounts.Add(account);

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return AccountResponse.From(account);
    }

    public Task<Result<AccountResponse>> DepositAsync(Guid accountId, decimal amount,
        CancellationToken cancellationToken = default) =>
        ExecuteAsync(accountId, account => account.Deposit(amount), cancellationToken);

    public Task<Result<AccountResponse>> WithdrawAsync(Guid accountId, decimal amount,
        CancellationToken cancellationToken = default) =>
        ExecuteAsync(accountId, account => account.Withdraw(amount), cancellationToken);

    public Task<Result<AccountResponse>> CloseAsync(Guid accountId, CancellationToken cancellationToken = default) =>
        ExecuteAsync(accountId, account => account.Close(), cancellationToken);

    private async Task<Result<AccountResponse>> ExecuteAsync(
        Guid accountId,
        Func<Account, Result> operation,
        CancellationToken cancellationToken)
    {
        var account = await accounts.GetByIdAsync(accountId, cancellationToken);
        if (account is null)
            return AccountErrors.NotFound(accountId);

        var result = operation(account);
        if (result.IsFailure)
            return result.Error!;

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return AccountResponse.From(account);
    }
}
