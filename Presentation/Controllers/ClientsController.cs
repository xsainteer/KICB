using Application.Clients;
using Application.Common;
using Microsoft.AspNetCore.Mvc;
using Presentation.Contracts;

namespace Presentation.Controllers;

[Route("api/clients")]
public sealed class ClientsController(ClientService clientService) : ApiController
{
    /// <summary>Lists clients, filtered and paged. Text filters match case-insensitively by substring.</summary>
    [HttpGet]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PagedResult<ClientResponse>>> List([FromQuery] ClientListQuery query,
        CancellationToken cancellationToken) =>
        Ok(await clientService.ListAsync(query.ToFilter(), query.Page, query.PageSize, cancellationToken));

    [HttpGet("{clientId:guid}")]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ClientResponse>> Get(Guid clientId, CancellationToken cancellationToken) =>
        FromResult(await clientService.GetAsync(clientId, cancellationToken));

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ClientResponse>> Create(ClientRequest request,
        CancellationToken cancellationToken) =>
        CreatedFromResult(
            await clientService.CreateAsync(request.ToDetails(), cancellationToken),
            nameof(Get),
            client => new { clientId = client.Id });

    [HttpPut("{clientId:guid}")]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ClientResponse>> Update(Guid clientId, ClientRequest request,
        CancellationToken cancellationToken) =>
        FromResult(await clientService.UpdateAsync(clientId, request.ToDetails(), cancellationToken));

    /// <summary>Deletes a client and their phones. Fails with 409 if the client has accounts.</summary>
    [HttpDelete("{clientId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Delete(Guid clientId, CancellationToken cancellationToken) =>
        NoContentFromResult(await clientService.DeleteAsync(clientId, cancellationToken));
}
