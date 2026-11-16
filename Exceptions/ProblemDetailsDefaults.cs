using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace StockMarkets.Exceptions;

/// <summary>
/// One place that decorates every ProblemDetails we produce (handler, model validation,
/// status-code pages) with the same extension members: "code" and "traceId".
/// </summary>
public static class ProblemDetailsDefaults
{
    public static void Apply(HttpContext httpContext, ProblemDetails problem)
    {
        problem.Extensions["traceId"] = Activity.Current?.Id ?? httpContext.TraceIdentifier;

        // Only fill "code" when the producer did not set a more specific one.
        if (!problem.Extensions.ContainsKey("code") && DefaultCodeFor(problem.Status) is { } code)
            problem.Extensions["code"] = code;
    }

    private static string? DefaultCodeFor(int? status) => status switch
    {
        StatusCodes.Status400BadRequest => ErrorCodes.ValidationError,
        StatusCodes.Status401Unauthorized => ErrorCodes.Unauthenticated,
        StatusCodes.Status403Forbidden => ErrorCodes.Forbidden,
        StatusCodes.Status404NotFound => ErrorCodes.NotFound,
        StatusCodes.Status500InternalServerError => ErrorCodes.InternalError,
        _ => null
    };
}
