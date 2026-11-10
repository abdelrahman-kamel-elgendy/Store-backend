using System.ComponentModel.DataAnnotations;
using Store.Core.Validation;

namespace Store.Core.DTO.Category;

public record UpdateCategoryRequest(
        [Required(AllowEmptyStrings = false, ErrorMessage = "Category name is required."),
        StringLength(100, MinimumLength = 2, ErrorMessage = "Category name must be between 2 and 100 characters.")]
        string Name,

        [Required(AllowEmptyStrings = false, ErrorMessage = "Category description is required."),
        StringLength(500, MinimumLength = 10, ErrorMessage = "Category description must be between 10 and 500 characters.")]
        string Description,

        [Range(1, int.MaxValue, ErrorMessage = "Parent Category ID must be greater than zero.")]
        int? ParentCategoryId

        ) : IValidatableObject
{
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var errors = new List<ValidationResult>();
        ValidationRules.AddRequiredText(errors, Name, nameof(Name), "Category name is required.", 2, 100);
        ValidationRules.AddRequiredText(errors, Description, nameof(Description), "Category description is required.", 10, 500);
        return errors;
    }
}
