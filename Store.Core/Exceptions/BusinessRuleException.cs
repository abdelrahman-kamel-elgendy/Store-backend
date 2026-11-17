namespace Store.Core.Exceptions;

/// <summary>The request violates a catalog rule that is not a field validation error.</summary>
public sealed class BusinessRuleException(string message) : Exception(message);
