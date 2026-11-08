using AutoMapper;
using Store.Core.DTO;
using Store.Core.Entities;
using Store.Core.Exceptions;
using Store.Core.Interfaces;

namespace Store.API.Services;

/// <summary>Implements the shared CRUD workflow for a mapped entity and its API DTOs.</summary>
/// <typeparam name="TEntity">The persisted entity type.</typeparam>
/// <typeparam name="TResponse">The response DTO returned to API callers.</typeparam>
/// <typeparam name="TCreateRequest">The create request DTO.</typeparam>
/// <typeparam name="TUpdateRequest">The update request DTO.</typeparam>
public class GenericService<TEntity, TResponse, TCreateRequest, TUpdateRequest>(
    IGenericRepository<TEntity> repository,
    IMapper mapper) : IGenericService<TResponse, TCreateRequest, TUpdateRequest>
    where TEntity : BaseEntity<int>
{
    protected virtual string EntityName => typeof(TEntity).Name; // gets the entity name used in not-found messages.

    public virtual async Task<PagedResult<TResponse>> GetAllAsync(int page, int pageSize)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(page, 1);
        ArgumentOutOfRangeException.ThrowIfLessThan(pageSize, 1);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(pageSize, 100);

        var skip = (int)Math.Min((long)(page - 1) * pageSize, int.MaxValue);
        var totalItems = await repository.CountAsync();
        var entities = await repository.GetPageAsync(skip, pageSize);

        return new PagedResult<TResponse>(
            mapper.Map<IReadOnlyList<TResponse>>(entities),
            page,
            pageSize,
            totalItems);
    }

    public virtual async Task<TResponse> GetByIdAsync(int id)
    {
        var entity = await repository.GetByIdAsync(id) ?? throw new NotFoundException($"{EntityName} with ID {id} was not found.");
        return mapper.Map<TResponse>(entity);
    }

    public virtual async Task<TResponse> CreateAsync(TCreateRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        var entity = mapper.Map<TEntity>(request);
        await repository.AddAsync(entity);

        return mapper.Map<TResponse>(entity);  // map after saving so Id/CreatedAt are filled
    }

    public virtual async Task<TResponse> UpdateAsync(int id, TUpdateRequest request)
    {
        var entity = await repository.GetByIdAsync(id) ?? throw new NotFoundException($"{EntityName} with ID {id} was not found.");
        mapper.Map(request, entity);
        entity.Id = id;
        await repository.UpdateAsync(entity);
        return mapper.Map<TResponse>(entity);
    }

    public virtual async Task DeleteAsync(int id)
    {
        try
        {
            await repository.DeleteAsync(id);
        }
        catch (NotFoundException)
        {
            throw new NotFoundException($"{EntityName} with ID {id} was not found.");
        }
    }

}
