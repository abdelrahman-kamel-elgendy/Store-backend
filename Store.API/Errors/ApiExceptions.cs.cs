namespace Store.API.Errors;

public sealed class ValidationException(string message, IReadOnlyDictionary<string, IEnumerable<string>> errors) : Exception(message)
{
    public IReadOnlyDictionary<string, IEnumerable<string>> Errors { get; } = errors;
}

public sealed class BadRequestException(string message) : Exception(message);

public sealed class NotFoundException(string message) : Exception(message);

public sealed class UnauthorizedException(string message) : Exception(message);

public sealed class ForbiddenException(string message) : Exception(message);

public sealed class ConflictException(string message) : Exception(message);

public sealed class BusinessRuleException(string message) : Exception(message);

public sealed class RateLimitException(string message) : Exception(message);

public sealed class ServiceUnavailableException(string message) : Exception(message);