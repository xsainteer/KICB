using Application.Accounts;
using Microsoft.AspNetCore.Mvc;
using Presentation.Contracts;

namespace Presentation.Controllers;

[Route("api/accounts")]
[ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
[ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
public sealed class AccountsController(AccountService accountService) : ApiController
{
    [HttpPost("{accountId:guid}/deposit")]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AccountResponse>> Deposit(Guid accountId, AmountRequest request,
        CancellationToken cancellationToken) =>
        FromResult(await accountService.DepositAsync(accountId, request.Amount, cancellationToken));

    [HttpPost("{accountId:guid}/withdraw")]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AccountResponse>> Withdraw(Guid accountId, AmountRequest request,
        CancellationToken cancellationToken) =>
        FromResult(await accountService.WithdrawAsync(accountId, request.Amount, cancellationToken));

    [HttpPost("{accountId:guid}/close")]
    public async Task<ActionResult<AccountResponse>> Close(Guid accountId, CancellationToken cancellationToken) =>
        FromResult(await accountService.CloseAsync(accountId, cancellationToken));

    [HttpPost("{accountId:guid}/cards/{cardId:guid}/block")]
    public async Task<ActionResult<CardResponse>> BlockCard(Guid accountId, Guid cardId,
        CancellationToken cancellationToken) =>
        FromResult(await accountService.BlockCardAsync(accountId, cardId, cancellationToken));

    [HttpPost("{accountId:guid}/cards/{cardId:guid}/unblock")]
    public async Task<ActionResult<CardResponse>> UnblockCard(Guid accountId, Guid cardId,
        CancellationToken cancellationToken) =>
        FromResult(await accountService.UnblockCardAsync(accountId, cardId, cancellationToken));
}
