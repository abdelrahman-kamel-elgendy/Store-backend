
using Store.Core.DTOs;

namespace Store.Core.Interfaces.Services;

/// <summary>CRUD use cases for a resource, expressed with DTOs rather than entities.</summary>
/// <typeparam name="TDto">Response DTO.</typeparam>
/// <typeparam name="TRequest">Request DTO.</typeparam>
public interface ICRUDService<TDto, in TRequest>
{
    /// <summary>Creates a resource.</summary>
    /// <param name="request">Creation payload.</param>
    Task<TDto> CreateAsync(TRequest request);

    /// <summary>Returns a page of resources.</summary>
    /// <param name="page">Page number.</param>
    /// <param name="pageSize">number of entities in one page.</param>
    /// <param name="includeDeleted">When true, a soft-deleted resource can be returned.</param>
    Task<PagedResult<TDto>> GetAllAsync(int page, int pageSize, bool includeDeleted = false);

    /// <summary>Returns one resource.</summary>
    /// <param name="id">Resource id.</param>
    /// <param name="includeDeleted">When true, a soft-deleted resource can be returned.</param>
    Task<TDto> GetByIdAsync(int id, bool includeDeleted = false);

    /// <summary>Updates a resource.</summary>
    /// <param name="id">Resource id.</param>
    /// <param name="request">Replacement payload.</param>
    Task<TDto> UpdateAsync(int id, TRequest request);

    /// <summary>Soft-deletes a resource.</summary>
    /// <param name="id">Resource id.</param>
    Task DeleteAsync(int id);

    /// <summary>Restores a soft-deleted resource.</summary>
    /// <param name="id">Resource id.</param>
    Task<TDto> RestoreAsync(int id);
}
