namespace StockMarkets.Exceptions;

/// <summary>
/// Base type for expected business-rule failures raised by the service layer.
/// <see cref="GlobalExceptionHandler"/> maps each subtype to an HTTP status code and a ProblemDetails body.
/// </summary>
public abstract class DomainException : Exception
{
    protected DomainException(string code, string message) : base(message)
    {
        Code = code;
    }

    /// <summary>Stable machine-readable code, see <see cref="ErrorCodes"/>.</summary>
    public string Code { get; }
}

/// <summary>The requested resource does not exist (HTTP 404).</summary>
public sealed class NotFoundException(string message) : DomainException(ErrorCodes.NotFound, message);

/// <summary>The operation conflicts with existing state, e.g. a duplicate (HTTP 409).</summary>
public sealed class ConflictException(string code, string message) : DomainException(code, message);

/// <summary>The caller's identity cannot be resolved to an existing user (HTTP 401).</summary>
public sealed class UnauthenticatedException(string message) : DomainException(ErrorCodes.Unauthenticated, message);

/// <summary>The input is invalid; carries per-field messages (HTTP 400).</summary>
public sealed class ValidationFailedException : DomainException
{
    public ValidationFailedException(IDictionary<string, string[]> errors)
        : base(ErrorCodes.ValidationError, "One or more validation errors occurred.")
    {
        Errors = errors;
    }

    public ValidationFailedException(string field, string message)
        : this(new Dictionary<string, string[]> { [field] = [message] })
    {
    }

    public IDictionary<string, string[]> Errors { get; }
}
