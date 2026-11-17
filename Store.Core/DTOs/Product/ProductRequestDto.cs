using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Store.Core.Common;
using Store.Core.Validation;

namespace Store.Core.DTOs.Product;

/// <summary>Payload used to create or replace a product.</summary>
/// <param name="Name">Display name. 2 to 100 characters.</param>
/// <param name="Description">Description. 10 to 1000 characters.</param>
/// <param name="NewPrice">Current price, from 0.01 to 9999999.99.</param>
/// <param name="OldPrice">Previous price, from 0.01 to 9999999.99.</param>
/// <param name="CategoryId">Existing category id.</param>
/// <param name="Images">Images to store. Null or empty leaves the product without images. On update, the supplied set replaces the previous set.</param>
public record ProductRequestDto(
    [Required(AllowEmptyStrings = false, ErrorMessage = "Product name is required."), NotWhitespace(ErrorMessage = "Product name is required."), StringLength(100, MinimumLength = 2, ErrorMessage = "Product name must be between 2 and 100 characters.")]
    string Name,

    [Required(AllowEmptyStrings = false, ErrorMessage = "Product description is required."), NotWhitespace(ErrorMessage = "Product description is required."), StringLength(1000, MinimumLength = 10, ErrorMessage = "Product description must be between 10 and 1000 characters.")]
    string Description,

    [Range(typeof(decimal), "0.01", "9999999.99", ErrorMessage = "Product new price must be between 0.01 and 9999999.99.")]
    decimal NewPrice,

    [Range(typeof(decimal), "0.01", "9999999.99", ErrorMessage = "Product old price must be between 0.01 and 9999999.99.")]
    decimal OldPrice,

    [Range(1, int.MaxValue, ErrorMessage = "Category id must be greater than zero.")]
    int CategoryId,

    IFormFileCollection? Images);
