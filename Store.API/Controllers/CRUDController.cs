using Microsoft.AspNetCore.Mvc;
using Store.API.Helper;
using Store.API.Services;
using Store.Core.DTOs;
using Store.Core.Interfaces.Services;

namespace Store.API.Controllers;

/// <summary>Base controller providing CRUD endpoints.</summary>
/// <typeparam name="TEntity">The domain entity type.</typeparam>
/// <typeparam name="TResponse">The response DTO returned by the service.</typeparam>
/// <typeparam name="TRequest">The request accepted by the API.</typeparam>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public abstract class CRUDController<TEntity, TResponse, TRequest>(ICRUDService<TResponse, TRequest> service, IApiResponseBuilder responseBuilder)
    : ControllerBase where TResponse : IIdentifiableDto
{
    protected virtual string ResourceName => typeof(TEntity).Name;
    protected virtual string ResourceCollectionName => $"{ResourceName}s";

    /// <summary>Creates a new resource.</summary>
    /// <param name="request">The resource payload to create.</param>
    /// <returns>The newly created resource, with a Location header pointing to its GetById endpoint.</returns>
    /// <response code="201">The resource was created successfully.</response>
    /// <response code="400">The request payload is invalid.</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public virtual async Task<ActionResult<ApiResponse<TResponse>>> Create([FromBody] TRequest request)
    {
        var response = await service.CreateAsync(request);
        var successResponse = responseBuilder.BuildSuccessResponse(response, $"{ResourceName} created successfully.");
        return CreatedAtAction(nameof(GetById), new { id = response.Id }, successResponse);
    }

    /// <summary>Retrieves a paginated list of resources.</summary>
    /// <param name="pagination">Page number and page size for pagination.</param>
    /// <param name="includeDeleted">When true, soft-deleted resources are included in the results.</param>
    /// <returns>A paginated collection of resources with pagination metadata.</returns>
    /// <response code="200">The collection was retrieved successfully (may be empty).</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public virtual async Task<ActionResult<ApiResponse<IReadOnlyList<TResponse>>>> Get([FromQuery] PaginationQuery pagination, [FromQuery] bool includeDeleted = false)
    {
        var result = await service.GetAllAsync(pagination.Page, pagination.PageSize, includeDeleted);
        var metadata = new ApiMetadata(Pagination: PaginationMetadata.Create(result.Page, result.PageSize, result.TotalItems));
        var message = result.TotalItems == 0 ? $"No {ResourceCollectionName} were found." : $"{ResourceCollectionName} retrieved successfully.";
        return Ok(responseBuilder.BuildSuccessResponse(result.Items, message, metadata));
    }

    /// <summary>Retrieves a single resource by its identifier.</summary>
    /// <param name="id">The unique identifier of the resource.</param>
    /// <param name="includeDeleted">When true, the resource is returned even if soft-deleted.</param>
    /// <returns>The requested resource.</returns>
    /// <response code="200">The resource was retrieved successfully.</response>
    /// <response code="404">No resource exists with the specified identifier.</response>
    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public virtual async Task<ActionResult<ApiResponse<TResponse>>> GetById(int id, [FromQuery] bool includeDeleted = false)
    {
        var response = await service.GetByIdAsync(id, includeDeleted);
        return Ok(responseBuilder.BuildSuccessResponse(response, $"{ResourceName} with ID {id} retrieved."));
    }

    /// <summary>Updates an existing resource.</summary>
    /// <param name="id">The unique identifier of the resource to update.</param>
    /// <param name="request">The updated resource payload.</param>
    /// <returns>The updated resource.</returns>
    /// <response code="200">The resource was updated successfully.</response>
    /// <response code="400">The request payload is invalid.</response>
    /// <response code="404">No resource exists with the specified identifier.</response>
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public virtual async Task<ActionResult<ApiResponse<TResponse>>> Update(int id, [FromBody] TRequest request)
    {
        var response = await service.UpdateAsync(id, request);
        return Ok(responseBuilder.BuildSuccessResponse(response, $"{ResourceName} updated successfully."));
    }

    /// <summary>Soft-deletes a resource.</summary>
    /// <param name="id">The unique identifier of the resource to delete.</param>
    /// <returns>No content on success.</returns>
    /// <response code="204">The resource was deleted successfully.</response>
    /// <response code="404">No resource exists with the specified identifier.</response>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public virtual async Task<IActionResult> Delete(int id)
    {
        await service.DeleteAsync(id);
        return NoContent();
    }

    /// <summary>Restores a previously soft-deleted resource.</summary>
    /// <param name="id">The unique identifier of the resource to restore.</param>
    /// <returns>The restored resource.</returns>
    /// <response code="200">The resource was restored successfully.</response>
    /// <response code="404">No resource exists with the specified identifier.</response>
    [HttpPatch("{id:int}/restore")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public virtual async Task<ActionResult<ApiResponse<TResponse>>> Restore(int id)
    {
        var response = await service.RestoreAsync(id);
        return Ok(responseBuilder.BuildSuccessResponse(response, $"{ResourceName} restored successfully."));
    }
}