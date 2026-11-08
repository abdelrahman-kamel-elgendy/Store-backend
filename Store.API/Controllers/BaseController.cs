using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Store.API.DTO;
using Store.API.Helper;
using Store.Core.Interfaces;

namespace Store.API.Controllers;

/// <summary>Provides shared paginated CRUD endpoints for integer-keyed resources.</summary>
/// <typeparam name="TResponse">The response DTO returned by the service.</typeparam>
/// <typeparam name="TCreateRequest">The create request accepted by the API.</typeparam>
/// <typeparam name="TUpdateRequest">The update request accepted by the API.</typeparam>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public abstract class BaseController<TResponse, TCreateRequest, TUpdateRequest>(
    IGenericService<TResponse, TCreateRequest, TUpdateRequest> service) : ControllerBase
{
    protected virtual string ResourceName => typeof(TResponse).Name;
    protected virtual string ResourceCollectionName => $"{ResourceName}s";

    [HttpGet]
    [EndpointSummary("List resources using pagination")]
    [EndpointDescription("Returns a page of resources in the standard API response envelope.")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public virtual async Task<ActionResult<ApiResponse<IReadOnlyList<TResponse>>>> Get(
        [FromQuery] PaginationQuery pagination)
    {
        var result = await service.GetAllAsync(pagination.Page, pagination.PageSize);
        var metadata = new ApiMetadata(Pagination: PaginationMetadata.Create(result.Page, result.PageSize, result.TotalItems));
        var message = result.TotalItems == 0 ? $"No {ResourceCollectionName} were found." : $"{char.ToUpperInvariant(ResourceCollectionName[0])}{ResourceCollectionName[1..]} retrieved successfully.";

        return Ok(ApiResponses.Success(result.Items, message, metadata));
    }

    [HttpGet("{id:int}")]
    [EndpointSummary("Retrieve a resource by ID")]
    [EndpointDescription("Returns the requested resource in the standard API response envelope.")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public virtual async Task<ActionResult<ApiResponse<TResponse>>> GetById(int id)
    {
        var response = await service.GetByIdAsync(id);
        return Ok(ApiResponses.Success(response, $"{ResourceName} with ID {id} retrieved successfully."));
    }

    [HttpPost]
    [EndpointSummary("Create a resource")]
    [EndpointDescription("Creates a resource and returns it with a Location header.")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public virtual async Task<ActionResult<ApiResponse<TResponse>>> Create([FromBody] TCreateRequest request)
    {
        var response = await service.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), ApiResponses.Success(response, $"{ResourceName} created successfully."));
    }

    [HttpPut("{id:int}")]
    [EndpointSummary("Update a resource")]
    [EndpointDescription("Updates the resource identified by the route ID.")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public virtual async Task<ActionResult<ApiResponse<TResponse>>> Update(int id, [FromBody] TUpdateRequest request)
    {
        var response = await service.UpdateAsync(id, request);
        return Ok(ApiResponses.Success(response, $"{ResourceName} updated successfully."));
    }

    [HttpDelete("{id:int}")]
    [EndpointSummary("Delete a resource")]
    [EndpointDescription("Deletes the resource identified by the route ID and returns no content.")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public virtual async Task<IActionResult> Delete(int id)
    {
        await service.DeleteAsync(id);
        return NoContent();
    }
}
