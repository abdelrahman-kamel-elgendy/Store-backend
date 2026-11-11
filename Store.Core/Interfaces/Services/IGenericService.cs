using Store.Core.DTO;

namespace Store.Core.Interfaces.Servics;

/// <summary>Defines CRUD use cases for a resource using API DTOs rather than entities.</summary>
/// <typeparam name="TDto">The response DTO returned to callers.</typeparam>
/// <typeparam name="TRequest">The create request DTO.</typeparam>
public interface IGenericService<TDto, in TRequest>
{
    Task<TDto> CreateAsync(TRequest request);
    Task<PagedResult<TDto>> GetAllAsync(int page, int pageSize, bool includeDeleted = false);
    Task<TDto> GetByIdAsync(int id, bool includeDeleted = false);
    Task<TDto> UpdateAsync(int id, TRequest request);
    Task DeleteAsync(int id);
    Task<TDto> RestoreAsync(int id);
}
