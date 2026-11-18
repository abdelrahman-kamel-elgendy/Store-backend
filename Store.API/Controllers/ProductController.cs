using Microsoft.AspNetCore.Mvc;
using Store.API.DTO;
using Store.API.Helper;
using Store.API.Services;
using Store.Core.DTOs.Product;
using Store.Core.Entities;
using Store.Core.Interfaces.Services;

namespace Store.API.Controllers;

/// <summary>Manages products (create, read, update, soft-delete, restore).</summary>
[Route("api/[controller]")]
[ApiController]
public class ProductController(IProductService service, IApiResponseBuilder responseBuilder)
    : CRUDController<Product, ProductResponseDto, ProductRequestDto>(service, responseBuilder)
{
    private readonly IApiResponseBuilder _responseBuilder = responseBuilder;

    /// <summary>Retrieves a paginated list of products in a specific category selected by id.</summary>
    /// <param name="categoryId">Category id.</param>
    /// <param name="pagination">Page number and page size for pagination.</param>
    /// <param name="includeDeleted">When true, soft-deleted resources are included in the results.</param>
    /// <returns>A paginated collection of products in the specific category with pagination metadata.</returns>
    /// <response code="200">The collection was retrieved successfully (may be empty).</response>
    [HttpGet("Category-id/{categoryId:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public virtual async Task<ActionResult<ApiResponse<IReadOnlyList<ProductResponseDto>>>> GetByCategoryId(
        int categoryId, [FromQuery] PaginationQuery pagination, [FromQuery] bool includeDeleted = false)
    {
        var result = await service.GetByCategoryIdAsync(categoryId, pagination.Page, pagination.PageSize, includeDeleted);
        var metadata = new ApiMetadata(Pagination: PaginationMetadata.Create(result.Page, result.PageSize, result.TotalItems));
        return Ok(_responseBuilder.BuildSuccessResponse(result.Items, $"Products retrieved successfully.", metadata));
    }

    /// <summary>Retrieves a paginated list of products in a specific category selected by name.</summary>
    /// <param name="categoryName">Category name.</param>
    /// <param name="pagination">Page number and page size for pagination.</param>
    /// <param name="includeDeleted">When true, soft-deleted resources are included in the results.</param>
    /// <returns>A paginated collection of products in the specific category with pagination metadata.</returns>
    /// <response code="200">The collection was retrieved successfully (may be empty).</response>
    [HttpGet("Category-name/{categoryName}")] // <-- FIXED: Removed :string
    [ProducesResponseType(StatusCodes.Status200OK)]
    public virtual async Task<ActionResult<ApiResponse<IReadOnlyList<ProductResponseDto>>>> GetByCategoryName(
        string categoryName, [FromQuery] PaginationQuery pagination, [FromQuery] bool includeDeleted = false)
    {
        var result = await service.GetByCategoryNameAsync(categoryName, pagination.Page, pagination.PageSize, includeDeleted);
        var metadata = new ApiMetadata(Pagination: PaginationMetadata.Create(result.Page, result.PageSize, result.TotalItems));
        return Ok(_responseBuilder.BuildSuccessResponse(result.Items, $"Products retrieved successfully.", metadata));
    }
}