namespace Store.Core.Exceptions;

/// <summary>The caller is not authenticated. Reserved for the planned JWT bearer scheme.</summary>
public sealed class UnauthorizedException(string message) : Exception(message);
