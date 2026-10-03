using Application.Phones;
using Microsoft.AspNetCore.Mvc;
using Presentation.Contracts;

namespace Presentation.Controllers;

[Route("api/clients/{clientId:guid}/phones")]
[ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
public sealed class PhonesController(PhoneService phoneService) : ApiController
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<PhoneResponse>>> List(Guid clientId,
        CancellationToken cancellationToken) =>
        FromResult(await phoneService.ListAsync(clientId, cancellationToken));

    [HttpGet("{phoneId:guid}")]
    public async Task<ActionResult<PhoneResponse>> Get(Guid clientId, Guid phoneId,
        CancellationToken cancellationToken) =>
        FromResult(await phoneService.GetAsync(clientId, phoneId, cancellationToken));

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PhoneResponse>> Create(Guid clientId, PhoneRequest request,
        CancellationToken cancellationToken) =>
        CreatedFromResult(
            await phoneService.CreateAsync(clientId, request.Number, request.Type!.Value, cancellationToken),
            nameof(Get),
            phone => new { clientId, phoneId = phone.Id });

    [HttpPut("{phoneId:guid}")]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PhoneResponse>> Update(Guid clientId, Guid phoneId, PhoneRequest request,
        CancellationToken cancellationToken) =>
        FromResult(await phoneService.UpdateAsync(clientId, phoneId, request.Number, request.Type!.Value,
            cancellationToken));

    [HttpDelete("{phoneId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete(Guid clientId, Guid phoneId, CancellationToken cancellationToken) =>
        NoContentFromResult(await phoneService.DeleteAsync(clientId, phoneId, cancellationToken));
}
