
using System.ComponentModel.DataAnnotations;
using Store.Core.Validation;

namespace Store.Core.DTOs.Category;

/// <summary>
/// Payload used to create or update a category.
/// </summary>
/// <param name="Name">Display name. 2 to 100 characters.</param>
/// <param name="Description">Description. 10 to 500 characters.</param>
/// <param name="ParentCategoryId">Parent category id, or null for a root category.</param>
public record CategoryRequestDto(
    [Required(AllowEmptyStrings = false, ErrorMessage = "Category name is required."), NotWhitespace(ErrorMessage = "Category name is required."), StringLength(100, MinimumLength = 2, ErrorMessage = "Category name must be between 2 and 100 characters.")]
    string Name,

    [Required(AllowEmptyStrings = false, ErrorMessage = "Category description is required."), NotWhitespace(ErrorMessage = "Category description is required."), StringLength(500, MinimumLength = 10, ErrorMessage = "Category description must be between 10 and 500 characters.")]
    string Description,

    [Range(1, int.MaxValue, ErrorMessage = "Parent category id must be greater than zero.")]
    int? ParentCategoryId
);
