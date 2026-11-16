using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace StockMarkets.Exceptions;

/// <summary>
/// Builds the 400 response [ApiController] returns when model binding/validation fails:
/// camelCase field names (matching the JSON the client sent) plus "code" and "traceId".
/// </summary>
public static class ModelStateProblemFactory
{
    public static IActionResult Create(ActionContext context)
    {
        var errors = context.ModelState
            .Where(entry => entry.Value is { Errors.Count: > 0 })
            .ToDictionary(
                entry => ToFieldName(entry.Key),
                entry => entry.Value!.Errors
                    .Select(e => string.IsNullOrEmpty(e.ErrorMessage) ? "The value is invalid." : e.ErrorMessage)
                    .ToArray());

        var problem = new ValidationProblemDetails(errors)
        {
            Status = StatusCodes.Status400BadRequest,
            Title = "Validation failed"
        };
        problem.Extensions["code"] = ErrorCodes.ValidationError;
        ProblemDetailsDefaults.Apply(context.HttpContext, problem);

        return new BadRequestObjectResult(problem) { ContentTypes = { "application/problem+json" } };
    }

    // "Password" -> "password", "$.title" -> "title"
    private static string ToFieldName(string key)
    {
        var name = key.StartsWith("$.") ? key[2..] : key;
        return name.Length == 0 ? name : char.ToLowerInvariant(name[0]) + name[1..];
    }
}
