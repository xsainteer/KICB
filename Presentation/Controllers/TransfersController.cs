using Application.Accounts;
using Application.Transfers;
using Microsoft.AspNetCore.Mvc;
using Presentation.Contracts;

namespace Presentation.Controllers;

[Route("api/transfers")]
public sealed class TransfersController(TransferService transferService) : ApiController
{
    /// <summary>Transfers funds between two accounts and returns the source account after the transfer.</summary>
    [HttpPost]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<AccountResponse>> Transfer(TransferRequest request,
        CancellationToken cancellationToken) =>
        FromResult(await transferService.TransferAsync(
            request.SourceAccountId, request.DestinationAccountId, request.Amount, cancellationToken));
}
