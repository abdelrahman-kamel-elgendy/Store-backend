namespace Store.Core.Exceptions;

/// <summary>The caller is authenticated but is not allowed to perform the action.</summary>
public sealed class ForbiddenException(string message) : Exception(message);
