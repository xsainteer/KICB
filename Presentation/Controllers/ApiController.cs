using Domain.Common;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers;

[ApiController]
public abstract class ApiController : ControllerBase
{
    /// <summary>
    /// Turns a <see cref="Result{T}"/> into 200 OK with its value, or into a ProblemDetails response for its error.
    /// </summary>
    protected ActionResult<T> FromResult<T>(Result<T> result) =>
        result.IsSuccess ? Ok(result.Value) : ErrorResult(result.Error!);

    /// <summary>
    /// Turns a <see cref="Result{T}"/> of a create operation into 201 Created, with a Location header pointing
    /// to <paramref name="actionName"/>, or into a ProblemDetails response for its error.
    /// </summary>
    protected ActionResult<T> CreatedFromResult<T>(Result<T> result, string actionName, Func<T, object> routeValues) =>
        result.IsSuccess
            ? CreatedAtAction(actionName, routeValues(result.Value), result.Value)
            : ErrorResult(result.Error!);

    /// <summary>Turns a <see cref="Result"/> into 204 No Content, or into a ProblemDetails response for its error.</summary>
    protected IActionResult NoContentFromResult(Result result)
    {
        if (result.IsFailure)
            return ErrorResult(result.Error!);

        return NoContent();
    }

    /// <summary>
    /// Maps a business <see cref="Error"/> to ProblemDetails: the status comes from its <see cref="ErrorType"/>,
    /// and its stable <see cref="Error.Code"/> is exposed as the "code" extension for clients to branch on.
    /// </summary>
    private ObjectResult ErrorResult(Error error)
    {
        var statusCode = error.Type switch
        {
            ErrorType.Validation => StatusCodes.Status400BadRequest,
            ErrorType.NotFound => StatusCodes.Status404NotFound,
            ErrorType.Conflict => StatusCodes.Status409Conflict,
            _ => StatusCodes.Status500InternalServerError
        };

        var problem = ProblemDetailsFactory.CreateProblemDetails(HttpContext, statusCode, detail: error.Message);
        problem.Extensions["code"] = error.Code;

        return new ObjectResult(problem) { StatusCode = statusCode };
    }
}
