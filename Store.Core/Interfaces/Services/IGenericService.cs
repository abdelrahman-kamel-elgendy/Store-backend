using Store.Core.Common;
using Store.Core.DTOs;

namespace Store.Core.Interfaces.Services;

/// <summary>CRUD use cases for a resource, expressed with DTOs rather than entities.</summary>
/// <typeparam name="TDto">Response DTO.</typeparam>
/// <typeparam name="TRequest">Request DTO.</typeparam>
public interface IGenericService<TDto, in TRequest>
{
    /// <summary>Creates a resource.</summary>
    /// <param name="request">Creation payload.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    Task<TDto> CreateAsync(TRequest request, CancellationToken cancellationToken = default);

    /// <summary>Returns a page of resources.</summary>
    /// <param name="query">Paging, search, and sort.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    Task<PagedResult<TDto>> GetAllAsync(ListQuery query, CancellationToken cancellationToken = default);

    /// <summary>Returns one resource.</summary>
    /// <param name="id">Resource id.</param>
    /// <param name="includeDeleted">When true, a soft-deleted resource can be returned.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    Task<TDto> GetByIdAsync(int id, bool includeDeleted = false, CancellationToken cancellationToken = default);

    /// <summary>Updates a resource.</summary>
    /// <param name="id">Resource id.</param>
    /// <param name="request">Replacement payload.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    Task<TDto> UpdateAsync(int id, TRequest request, CancellationToken cancellationToken = default);

    /// <summary>Soft-deletes a resource.</summary>
    /// <param name="id">Resource id.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>Restores a soft-deleted resource.</summary>
    /// <param name="id">Resource id.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    Task<TDto> RestoreAsync(int id, CancellationToken cancellationToken = default);
}
