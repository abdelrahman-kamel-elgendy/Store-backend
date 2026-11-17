using System.ComponentModel.DataAnnotations;

namespace Store.Core.Validation;

/// <summary>Rejects a string that contains only whitespace.</summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter)]
public sealed class NotWhitespaceAttribute : ValidationAttribute
{
    /// <summary>Validates that a non-empty string contains a non-whitespace character.</summary>
    /// <param name="value">The value to validate.</param>
    /// <param name="validationContext">The validation context.</param>
    /// <returns>Success, or a failure when the value is whitespace.</returns>
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is string text && text.Length > 0 && string.IsNullOrWhiteSpace(text))
            return new ValidationResult(ErrorMessage ?? $"{validationContext.DisplayName} is required.");

        return ValidationResult.Success;
    }
}
