using Microsoft.AspNetCore.Mvc;
using Store.API.DTO;
using Store.API.Helper;
using Store.Core.DTO;
using Store.Core.Interfaces.Servics;

namespace Store.API.Controllers;

/// <summary>Provides shared paginated CRUD endpoints for integer-keyed resources.</summary>
/// <typeparam name="TResponse">The response DTO returned by the service.</typeparam>
/// <typeparam name="TRequest">The request accepted by the API.</typeparam>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public abstract class BaseController<TEntity, TResponse, TRequest>(IGenericService<TResponse, TRequest> service) : ControllerBase where TResponse : IIdentifiableDto
{
    protected virtual string ResourceName => typeof(TEntity).Name;
    protected virtual string ResourceCollectionName => $"{ResourceName}s";

    [HttpPost]
    public virtual async Task<ActionResult<ApiResponse<TResponse>>> Create([FromBody] TRequest request)
    {
        var response = await service.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = response.Id }, ApiResponses.Success(response, $"{ResourceName} created successfully."));
    }

    [HttpGet]
    public virtual async Task<ActionResult<ApiResponse<IReadOnlyList<TResponse>>>> Get(
        [FromQuery] PaginationQuery pagination,
        [FromQuery] bool includeDeleted = false)
    {
        var result = await service.GetAllAsync(pagination.Page, pagination.PageSize, includeDeleted);
        var metadata = new ApiMetadata(Pagination: PaginationMetadata.Create(result.Page, result.PageSize, result.TotalItems));
        var message = result.TotalItems == 0 ? $"No {ResourceCollectionName} were found." : $"{char.ToUpperInvariant(ResourceCollectionName[0])}{ResourceCollectionName[1..]} retrieved successfully.";

        return Ok(ApiResponses.Success(result.Items, message, metadata));
    }

    [HttpGet("{id:int}")]
    public virtual async Task<ActionResult<ApiResponse<TResponse>>> GetById(
        int id,
        [FromQuery] bool includeDeleted = false)
    {
        var response = await service.GetByIdAsync(id, includeDeleted);
        return Ok(ApiResponses.Success(response, $"{ResourceName} with ID {id} retrieved successfully."));
    }

    [HttpPut("{id:int}")]
    public virtual async Task<ActionResult<ApiResponse<TResponse>>> Update(int id, [FromBody] TRequest request)
    {
        var response = await service.UpdateAsync(id, request);
        return Ok(ApiResponses.Success(response, $"{ResourceName} updated successfully."));
    }

    [HttpDelete("{id:int}")]
    public virtual async Task<IActionResult> Delete(int id)
    {
        await service.DeleteAsync(id);
        return NoContent();
    }

    [HttpPatch("{id:int}/restore")]
    public virtual async Task<ActionResult<ApiResponse<TResponse>>> Restore(int id)
    {
        var response = await service.RestoreAsync(id);
        return Ok(ApiResponses.Success(response, $"{ResourceName} with ID {id} restored successfully."));
    }
}
