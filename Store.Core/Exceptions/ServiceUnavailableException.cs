namespace Store.Core.Exceptions;

/// <summary>A required dependency is temporarily unavailable.</summary>
public sealed class ServiceUnavailableException(string message) : Exception(message);
