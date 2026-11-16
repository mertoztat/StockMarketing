namespace StockMarkets.Exceptions;

/// <summary>
/// Base type for expected business-rule failures raised by the service layer.
/// The exception handler maps each subtype to an HTTP status code.
/// </summary>
public abstract class DomainException : Exception
{
    protected DomainException(string message) : base(message)
    {
    }
}

/// <summary>The requested resource does not exist (HTTP 404).</summary>
public sealed class NotFoundException(string message) : DomainException(message);

/// <summary>The operation conflicts with existing state, e.g. a duplicate (HTTP 409).</summary>
public sealed class ConflictException(string message) : DomainException(message);

/// <summary>The caller's identity cannot be resolved to an existing user (HTTP 401).</summary>
public sealed class UnauthenticatedException(string message) : DomainException(message);
