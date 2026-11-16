using Microsoft.AspNetCore.Diagnostics;

namespace StockMarkets.Exceptions;

/// <summary>
/// Turns <see cref="DomainException"/>s into HTTP responses.
/// Interim version that keeps the pre-refactor response shapes; TASK-002 replaces the body with ProblemDetails.
/// </summary>
public sealed class DomainExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        if (exception is not DomainException domainException)
            return false; // Not ours: let the next handler / default pipeline deal with it.

        httpContext.Response.StatusCode = domainException switch
        {
            NotFoundException => StatusCodes.Status404NotFound,
            ConflictException => StatusCodes.Status409Conflict,
            UnauthenticatedException => StatusCodes.Status401Unauthorized,
            _ => StatusCodes.Status500InternalServerError
        };

        // 401 used to return an empty body; 404/409 returned the message as a JSON string.
        if (domainException is not UnauthenticatedException)
            await httpContext.Response.WriteAsJsonAsync(domainException.Message, cancellationToken);

        return true;
    }
}
