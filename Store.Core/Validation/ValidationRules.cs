using System.ComponentModel.DataAnnotations;

namespace Store.Core.Validation;

public static class ValidationRules
{
    public static void AddRequiredText(
        ICollection<ValidationResult> results,
        string? value,
        string memberName,
        string message,
        int minLength = 1,
        int? maxLength = null)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            results.Add(new ValidationResult(message, [memberName]));
            return;
        }

        var trimmed = value.Trim();
        if (trimmed.Length < minLength)
        {
            results.Add(new ValidationResult(message, [memberName]));
            return;
        }

        if (maxLength.HasValue && trimmed.Length > maxLength.Value)
        {
            results.Add(new ValidationResult(message, [memberName]));
        }
    }

    public static void AddPositiveId(
        ICollection<ValidationResult> results,
        int value,
        string memberName,
        string message)
    {
        if (value <= 0)
            results.Add(new ValidationResult(message, [memberName]));
    }

    public static void AddPositiveDecimal(
        ICollection<ValidationResult> results,
        decimal value,
        string memberName,
        string message,
        decimal minimum = 0.01m)
    {
        if (value <= minimum)
            results.Add(new ValidationResult(message, [memberName]));
    }

}
