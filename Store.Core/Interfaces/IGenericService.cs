namespace Store.Core.Interfaces;

using Store.Core.DTO;

/// <summary>Defines CRUD use cases for a resource using API DTOs rather than entities.</summary>
/// <typeparam name="TDto">The response DTO returned to callers.</typeparam>
/// <typeparam name="TCreateRequest">The create request DTO.</typeparam>
/// <typeparam name="TUpdateRequest">The update request DTO.</typeparam>
public interface IGenericService<TDto, in TCreateRequest, in TUpdateRequest>
{
    Task<PagedResult<TDto>> GetAllAsync(int page, int pageSize);
    Task<TDto> GetByIdAsync(int id);
    Task<TDto> CreateAsync(TCreateRequest request);
    Task<TDto> UpdateAsync(int id, TUpdateRequest request);
    Task DeleteAsync(int id);
}
