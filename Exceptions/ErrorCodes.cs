namespace StockMarkets.Exceptions;

/// <summary>
/// Stable machine-readable error codes (the "code" field of every error response).
/// Clients branch on these, never on the human-readable message. See docs/architecture/api/stockmarkets.md.
/// </summary>
public static class ErrorCodes
{
    public const string ValidationError = "VALIDATION_ERROR";
    public const string Unauthenticated = "UNAUTHENTICATED";
    public const string Forbidden = "FORBIDDEN";
    public const string NotFound = "NOT_FOUND";
    public const string DuplicateSymbol = "DUPLICATE_SYMBOL";
    public const string DuplicateComment = "DUPLICATE_COMMENT";
    public const string AlreadyInPortfolio = "ALREADY_IN_PORTFOLIO";
    public const string UserExists = "USER_EXISTS";
    public const string InternalError = "INTERNAL_ERROR";
}
