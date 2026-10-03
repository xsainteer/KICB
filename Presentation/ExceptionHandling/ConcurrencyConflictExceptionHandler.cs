using Application.Common;
using Microsoft.AspNetCore.Diagnostics;

namespace Presentation.ExceptionHandling;

/// <summary>
/// Turns a <see cref="ConcurrencyConflictException"/> into 409 Conflict: the client should reload and retry.
/// Every other unhandled exception falls through to the default 500 ProblemDetails response.
/// </summary>
public sealed class ConcurrencyConflictExceptionHandler(IProblemDetailsService problemDetailsService)
    : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception,
        CancellationToken cancellationToken)
    {
        if (exception is not ConcurrencyConflictException)
            return false;

        httpContext.Response.StatusCode = StatusCodes.Status409Conflict;

        return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            Exception = exception,
            ProblemDetails =
            {
                Status = StatusCodes.Status409Conflict,
                Title = "Concurrency conflict",
                Detail = exception.Message
            }
        });
    }
}
