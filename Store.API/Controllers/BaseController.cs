using System;
using Microsoft.AspNetCore.Mvc;
using Store.API.DTO;
using Store.API.Helper;
using Store.API.Services;
using Store.Core.DTOs;
using Store.Core.Interfaces.Services;

namespace Store.API.Controllers;

/// <summary>Provides shared paginated CRUD endpoints for integer-keyed resources.</summary>
/// <typeparam name="TEntity">The entity type, used only for response messages.</typeparam>
/// <typeparam name="TResponse">The response DTO returned by the service.</typeparam>
/// <typeparam name="TRequest">The request accepted by the API.</typeparam>
/// <typeparam name="TFilter">The query-string filter bound for list requests.</typeparam>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public abstract class BaseController<TEntity, TResponse, TRequest, TFilter>(IBaseService<TResponse, TRequest, TFilter> service, IApiResponseBuilder responseBuilder) : ControllerBase
    where TResponse : IIdentifiableDto
    where TFilter : IFilter
{
    protected virtual string ResourceName => typeof(TEntity).Name;
    protected virtual string ResourceCollectionName => $"{ResourceName}s";

    [HttpPost]
    public virtual async Task<ActionResult<ApiResponse<TResponse>>> Create([FromBody] TRequest request)
    {
        var response = await service.CreateAsync(request);
        var successResponse = responseBuilder.BuildSuccessResponse(response, $"{ResourceName} created successfully.");
        return CreatedAtAction(nameof(GetById), new { id = response.Id }, successResponse);
    }

    [HttpGet]
    public virtual async Task<ActionResult<ApiResponse<IReadOnlyList<TResponse>>>> Get(
        [FromQuery] PaginationQuery pagination,
        [FromQuery] TFilter filter)
    {
        var result = await service.GetAllPagedAsync(filter, pagination.Page, pagination.PageSize);
        var metadata = new ApiMetadata(Pagination: PaginationMetadata.Create(result.Page, result.PageSize, result.TotalItems));
        var message = result.TotalItems == 0
            ? $"No {ResourceCollectionName} were found."
            : $"{char.ToUpperInvariant(ResourceCollectionName[0])}{ResourceCollectionName[1..]} retrieved successfully.";

        return Ok(responseBuilder.BuildSuccessResponse(result.Items, message, metadata));
    }

    [HttpGet("{id:int}")]
    public virtual async Task<ActionResult<ApiResponse<TResponse>>> GetById(
        int id, [FromQuery] bool includeDeleted = false)
    {
        var response = await service.GetByIdAsync(id, includeDeleted);
        return Ok(responseBuilder.BuildSuccessResponse(response, $"{ResourceName} with ID {id} retrieved."));
    }

    [HttpPut("{id:int}")]
    public virtual async Task<ActionResult<ApiResponse<TResponse>>> Update(int id, [FromBody] TRequest request)
    {
        var response = await service.UpdateAsync(id, request);
        return Ok(responseBuilder.BuildSuccessResponse(response, $"{ResourceName} updated successfully."));
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
        return Ok(responseBuilder.BuildSuccessResponse(response, $"{ResourceName} with ID {id} restored successfully."));
    }
}
