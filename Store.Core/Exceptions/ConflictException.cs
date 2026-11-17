namespace Store.Core.Exceptions;

/// <summary>The request conflicts with the current state of a resource.</summary>
public sealed class ConflictException(string message) : Exception(message);
