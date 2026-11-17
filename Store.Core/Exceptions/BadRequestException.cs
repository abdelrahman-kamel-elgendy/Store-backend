namespace Store.Core.Exceptions;

/// <summary>The request is syntactically valid but cannot be accepted.</summary>
public sealed class BadRequestException(string message) : Exception(message);
