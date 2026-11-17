namespace Store.Core.Exceptions;

/// <summary>Represents invalid request data with associated field errors.</summary>
/// <param name="message">Summary shown to the caller.</param>
/// <param name="errors">Field names mapped to one or more messages.</param>
public sealed class ValidationsException(string message, IReadOnlyDictionary<string, IEnumerable<string>> errors) : Exception(message)
{
    /// <summary>Field errors. An empty key means the error is not tied to one field.</summary>
    public IReadOnlyDictionary<string, IEnumerable<string>> Errors { get; } = errors;
}
