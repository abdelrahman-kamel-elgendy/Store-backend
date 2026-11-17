using System.ComponentModel.DataAnnotations;
using Store.Core.Exceptions;

namespace Store.Core.Validation;

/// <summary>Runs data annotations and raises <see cref="ValidationException"/>.</summary>
public static class ObjectValidator
{
    /// <summary>Validates every property on <paramref name="instance"/>.</summary>
    /// <param name="instance">The object to validate.</param>
    /// <exception cref="ValidationException">One or more annotations failed.</exception>
    public static void EnsureValid(object instance)
    {
        var results = new List<ValidationResult>();
        if (Validator.TryValidateObject(instance, new ValidationContext(instance), results, validateAllProperties: true))
            return;

        var errors = results
            .GroupBy(result => result.MemberNames.FirstOrDefault() ?? string.Empty)
            .ToDictionary(
                group => group.Key,
                group => group.Select(result => result.ErrorMessage ?? "The supplied value is invalid.").ToArray() as IEnumerable<string>);

        throw new ValidationsException("Request validation failed.", errors);
    }
}
