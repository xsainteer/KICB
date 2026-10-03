using Application.Accounts;
using Microsoft.AspNetCore.Mvc;
using Presentation.Contracts;

namespace Presentation.Controllers;

/// <summary>
/// Accounts are opened and listed under their client, and addressed directly by id afterwards.
/// There is no update or delete: number and currency are fixed, the balance changes only through operations,
/// and an account is closed rather than deleted.
/// </summary>
[Route("api/accounts")]
[ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
public sealed class AccountsController(AccountService accountService) : ApiController
{
    [HttpGet("/api/clients/{clientId:guid}/accounts")]
    public async Task<ActionResult<IReadOnlyList<AccountResponse>>> ListByClient(Guid clientId,
        CancellationToken cancellationToken) =>
        FromResult(await accountService.ListByClientAsync(clientId, cancellationToken));

    [HttpPost("/api/clients/{clientId:guid}/accounts")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AccountResponse>> Open(Guid clientId, OpenAccountRequest request,
        CancellationToken cancellationToken) =>
        CreatedFromResult(
            await accountService.OpenAsync(clientId, request.Currency!.Value, cancellationToken),
            nameof(Get),
            account => new { accountId = account.Id });

    [HttpGet("{accountId:guid}")]
    public async Task<ActionResult<AccountResponse>> Get(Guid accountId, CancellationToken cancellationToken) =>
        FromResult(await accountService.GetAsync(accountId, cancellationToken));

    [HttpPost("{accountId:guid}/deposit")]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<AccountResponse>> Deposit(Guid accountId, AmountRequest request,
        CancellationToken cancellationToken) =>
        FromResult(await accountService.DepositAsync(accountId, request.Amount, cancellationToken));

    [HttpPost("{accountId:guid}/withdraw")]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<AccountResponse>> Withdraw(Guid accountId, AmountRequest request,
        CancellationToken cancellationToken) =>
        FromResult(await accountService.WithdrawAsync(accountId, request.Amount, cancellationToken));

    [HttpPost("{accountId:guid}/close")]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<AccountResponse>> Close(Guid accountId, CancellationToken cancellationToken) =>
        FromResult(await accountService.CloseAsync(accountId, cancellationToken));
}
