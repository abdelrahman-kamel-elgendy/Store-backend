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

    /// <summary>Retrieves a paginated list of products.</summary>
    /// <param name="categoryId">When has value, only products in spacific category are returned.</param>
    /// <param name="pagination">Page number and page size for pagination.</param>
    /// <param name="includeDeleted">When true, soft-deleted resources are included in the results.</param>
    /// <returns>A paginated collection of products in the specific category with pagination metadata.</returns>
    /// <response code="200">The collection was retrieved successfully (may be empty).</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<ProductResponseDto>>>> Get(
        [FromQuery] PaginationQuery pagination,
        [FromQuery] bool includeDeleted = false,
        [FromQuery] int? categoryId = null)
    {
        var result = await service.GetPageAsync(categoryId, pagination.Page, pagination.PageSize, includeDeleted);
        var metadata = new ApiMetadata(Pagination: PaginationMetadata.Create(result.Page, result.PageSize, result.TotalItems));
        var message = result.TotalItems == 0 ? $"No {ResourceCollectionName} were found." : $"{ResourceCollectionName} retrieved successfully.";
        return Ok(_responseBuilder.BuildSuccessResponse(result.Items, message, metadata));
    }
}