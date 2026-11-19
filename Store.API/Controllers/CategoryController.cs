using Microsoft.AspNetCore.Mvc;
using Store.API.DTO;
using Store.API.Helper;
using Store.API.Services;
using Store.Core.DTOs.Category;
using Store.Core.Entities;
using Store.Core.Interfaces.Services;

namespace Store.API.Controllers;

/// <summary> Manages product categories (create, read, update, soft-delete, restore)./// </summary>
[Route("api/[controller]")]
[ApiController]
public class CategoryController(ICategoryService service, IApiResponseBuilder responseBuilder)
    : CRUDController<Category, CategoryResponseDto, CategoryRequestDto>(service, responseBuilder)
{
    private readonly IApiResponseBuilder _responseBuilder = responseBuilder;

    /// <summary>Retrieves a paginated list of Categories.</summary>
    /// <param name="pagination">Page number and page size for pagination.</param>
    /// <param name="includeDeleted">When true, soft-deleted resources are included in the results.</param>
    /// <returns>A paginated collection of resources with pagination metadata.</returns>
    /// <response code="200">The collection was retrieved successfully (may be empty).</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public virtual async Task<ActionResult<ApiResponse<IReadOnlyList<CategoryResponseDto>>>> Get([FromQuery] PaginationQuery pagination, [FromQuery] bool includeDeleted = false)
    {
        var result = await service.GetPageAsync(pagination.Page, pagination.PageSize, includeDeleted);
        var metadata = new ApiMetadata(Pagination: PaginationMetadata.Create(result.Page, result.PageSize, result.TotalItems));
        var message = result.TotalItems == 0 ? $"No {ResourceCollectionName} were found." : $"{ResourceCollectionName} retrieved successfully.";
        return Ok(_responseBuilder.BuildSuccessResponse(result.Items, message, metadata));
    }
}
