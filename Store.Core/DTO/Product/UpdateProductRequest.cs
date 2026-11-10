using System;
using System.ComponentModel.DataAnnotations;
using Store.Core.Validation;

namespace Store.Core.DTO.Product;

public record UpdateProductRequest(
     [Required(AllowEmptyStrings = false, ErrorMessage = "Product name is required."),
     StringLength(100, MinimumLength = 2, ErrorMessage = "Product name must be between 2 and 100 characters.")]
    string Name,

    [Required(AllowEmptyStrings = false, ErrorMessage = "Product description is required."),
    StringLength(1000, MinimumLength = 10, ErrorMessage = "Product description must be between 10 and 1000 characters.")]
    string Description,

    [Range(typeof(decimal), "0.01", "9999999.99", ErrorMessage = "Product price must be greater than zero.")]
    decimal Price,

    [Range(1, int.MaxValue, ErrorMessage = "Category ID must be greater than zero.")]
    int CategoryId,

    [Range(0, int.MaxValue, ErrorMessage = "Photo ID must be zero or a positive value.")]
    int? PhotoId = 0
    ) : IValidatableObject
{
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var errors = new List<ValidationResult>();
        ValidationRules.AddRequiredText(errors, Name, nameof(Name), "Product name is required.", 2, 100);
        ValidationRules.AddRequiredText(errors, Description, nameof(Description), "Product description is required.", 10, 1000);
        ValidationRules.AddPositiveDecimal(errors, Price, nameof(Price), "Product price must be greater than zero.");
        ValidationRules.AddPositiveId(errors, CategoryId, nameof(CategoryId), "Category ID must be greater than zero.");

        if (PhotoId < 0)
            errors.Add(new ValidationResult("Photo ID must be zero or a positive value.", [nameof(PhotoId)]));

        return errors;
    }
}


