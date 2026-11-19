using System;
using Store.Core.DTOs;

namespace Store.Core.Interfaces.Services;

/// <summary>Defines CRUD use cases for a resource using API DTOs rather than entities.</summary>
/// <typeparam name="TResponse">The response DTO returned to callers.</typeparam>
/// <typeparam name="TRequest">The create/update request DTO.</typeparam>
/// <typeparam name="TFilter">The criteria accepted when listing.</typeparam>
public interface IBaseService<TResponse, in TRequest, in TFilter> where TFilter : IFilter
{
    Task<TResponse> CreateAsync(TRequest request);
    Task<PagedResult<TResponse>> GetAllPagedAsync(TFilter filter, int page, int pageSize);
    Task<TResponse> GetByIdAsync(int id, bool includeDeleted = false);
    Task<TResponse> UpdateAsync(int id, TRequest request);
    Task DeleteAsync(int id);
    Task<TResponse> RestoreAsync(int id);
}