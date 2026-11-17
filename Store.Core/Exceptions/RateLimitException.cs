namespace Store.Core.Exceptions;

/// <summary>The caller exceeded the configured request limit.</summary>
public sealed class RateLimitException(string message) : Exception(message);
