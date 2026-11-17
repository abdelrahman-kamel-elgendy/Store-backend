namespace Store.Core.Exceptions;

/// <summary>The requested resource does not exist or is hidden by the soft-delete filter.</summary>
public sealed class NotFoundException(string message) : Exception(message);
