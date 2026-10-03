using Application.Cards;
using Microsoft.AspNetCore.Mvc;
using Presentation.Contracts;

namespace Presentation.Controllers;

/// <summary>
/// Cards of an account. There is no update or delete: a card is blocked rather than deleted.
/// </summary>
[Route("api/accounts/{accountId:guid}/cards")]
[ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
public sealed class CardsController(CardService cardService) : ApiController
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<CardResponse>>> List(Guid accountId,
        CancellationToken cancellationToken) =>
        FromResult(await cardService.ListAsync(accountId, cancellationToken));

    [HttpGet("{cardId:guid}")]
    public async Task<ActionResult<CardResponse>> Get(Guid accountId, Guid cardId,
        CancellationToken cancellationToken) =>
        FromResult(await cardService.GetAsync(accountId, cardId, cancellationToken));

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<CardResponse>> Issue(Guid accountId, IssueCardRequest request,
        CancellationToken cancellationToken) =>
        CreatedFromResult(
            await cardService.IssueAsync(accountId, request.Type!.Value, cancellationToken),
            nameof(Get),
            card => new { accountId, cardId = card.Id });

    [HttpPost("{cardId:guid}/block")]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<CardResponse>> Block(Guid accountId, Guid cardId,
        CancellationToken cancellationToken) =>
        FromResult(await cardService.BlockAsync(accountId, cardId, cancellationToken));

    [HttpPost("{cardId:guid}/unblock")]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<CardResponse>> Unblock(Guid accountId, Guid cardId,
        CancellationToken cancellationToken) =>
        FromResult(await cardService.UnblockAsync(accountId, cardId, cancellationToken));
}
