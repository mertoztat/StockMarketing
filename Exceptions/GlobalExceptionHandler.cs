using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace StockMarkets.Exceptions;

/// <summary>
/// Last line of defence for every exception thrown while handling a request:
/// expected <see cref="DomainException"/>s become 4xx, anything else becomes a generic 500
/// whose body never contains the exception message or stack trace.
/// </summary>
public sealed class GlobalExceptionHandler : IExceptionHandler
{
    private readonly IProblemDetailsService _problemDetailsService;
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(IProblemDetailsService problemDetailsService,
        ILogger<GlobalExceptionHandler> logger)
    {
        _problemDetailsService = problemDetailsService;
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var problem = exception is DomainException domainException
            ? CreateDomainProblem(domainException)
            : CreateUnexpectedProblem(httpContext, exception);

        httpContext.Response.StatusCode = problem.Status!.Value;
        return await _problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            ProblemDetails = problem
            // Exception is deliberately NOT passed: it keeps details out of the response.
        });
    }

    private static ProblemDetails CreateDomainProblem(DomainException exception)
    {
        ProblemDetails problem = exception switch
        {
            ValidationFailedException v => new ValidationProblemDetails(v.Errors)
            {
                Status = StatusCodes.Status400BadRequest, Title = "Validation failed", Detail = v.Message
            },
            NotFoundException => new ProblemDetails { Status = StatusCodes.Status404NotFound, Title = "Not found" },
            ConflictException => new ProblemDetails { Status = StatusCodes.Status409Conflict, Title = "Conflict" },
            UnauthenticatedException => new ProblemDetails
                { Status = StatusCodes.Status401Unauthorized, Title = "Unauthenticated" },
            _ => new ProblemDetails { Status = StatusCodes.Status500InternalServerError, Title = "Server error" }
        };

        problem.Detail ??= exception.Message;
        problem.Extensions["code"] = exception.Code;
        return problem;
    }

    private ProblemDetails CreateUnexpectedProblem(HttpContext httpContext, Exception exception)
    {
        // Structured log: the exception goes to the log, only route data (no query string, no body) is added.
        _logger.LogError(exception, "Unhandled exception on {Method} {Path}",
            httpContext.Request.Method, httpContext.Request.Path.Value);

        return new ProblemDetails
        {
            Status = StatusCodes.Status500InternalServerError,
            Title = "An unexpected error occurred",
            Detail = "Please try again later. If the problem persists, contact support with the trace id."
        };
    }
}
