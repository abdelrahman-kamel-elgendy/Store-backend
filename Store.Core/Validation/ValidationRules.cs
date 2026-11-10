using System.ComponentModel.DataAnnotations;

namespace Store.Core.Validation;

public static class ValidationRules
{
    public static void AddError(IDictionary<string, List<string>> errors, string field, string message)
    {
        if (string.IsNullOrWhiteSpace(field))
            throw new ArgumentException("Validation field name cannot be null or empty.", nameof(field));

        if (!errors.TryGetValue(field, out var values))
        {
            values = [];
            errors[field] = values;
        }

        values.Add(message);
    }

    public static IReadOnlyDictionary<string, IEnumerable<string>> ToReadOnlyDictionary(
        IDictionary<string, List<string>> errors)
    {
        return errors.ToDictionary(
            pair => pair.Key,
            pair => (IEnumerable<string>)pair.Value.ToArray());
    }

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
