using Store.Core.DTO;

namespace Store.Core.Interfaces.Servics;

/// <summary>Defines CRUD use cases for a resource using API DTOs rather than entities.</summary>
/// <typeparam name="TDto">The response DTO returned to callers.</typeparam>
/// <typeparam name="TCreateRequest">The create request DTO.</typeparam>
/// <typeparam name="TUpdateRequest">The update request DTO.</typeparam>
public interface IGenericService<TDto, in TCreateRequest, in TUpdateRequest>
{
    Task<TDto> CreateAsync(TCreateRequest request);
    Task<PagedResult<TDto>> GetAllAsync(int page, int pageSize);
    Task<TDto> GetByIdAsync(int id);
    Task<TDto> UpdateAsync(int id, TUpdateRequest request);
    Task DeleteAsync(int id);
    Task<TDto> RestoreAsync(int id);
}
