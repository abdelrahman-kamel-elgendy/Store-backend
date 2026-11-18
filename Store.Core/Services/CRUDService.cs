using AutoMapper;
using Store.Core.DTOs;
using Store.Core.Entities;
using Store.Core.Exceptions;
using Store.Core.Interfaces.Repositories;
using Store.Core.Interfaces.Services;

namespace Store.Service.Services;

/// <summary>Implements the shared CRUD workflow for a mapped entity and its API DTOs.</summary>
/// <typeparam name="TEntity">The persisted entity type.</typeparam>
/// <typeparam name="TResponse">The response DTO returned to API callers.</typeparam>
/// <typeparam name="TRequest">The request DTO.</typeparam>
public class CRUDService<TEntity, TResponse, TRequest>(ICRUDRepository<TEntity> repository, IMapper mapper)
    : ICRUDService<TResponse, TRequest> where TEntity : BaseEntity<int>
{
    protected virtual string EntityName => typeof(TEntity).Name; // gets the entity name used in not-found messages.

    public virtual async Task<TResponse> CreateAsync(TRequest request)
    {
        var entity = mapper.Map<TEntity>(request);
        await repository.AddAsync(entity);

        return mapper.Map<TResponse>(entity);
    }

    public virtual async Task<PagedResult<TResponse>> GetAllAsync(int page, int pageSize, bool includeDeleted = false)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(page, 1);
        ArgumentOutOfRangeException.ThrowIfLessThan(pageSize, 1);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(pageSize, 100);

        var skip = (int)Math.Min((long)(page - 1) * pageSize, int.MaxValue);
        var totalItems = await repository.CountAsync(includeDeleted);
        var entities = await repository.GetPageAsync(skip, pageSize, includeDeleted);

        return new PagedResult<TResponse>(
            mapper.Map<IReadOnlyList<TResponse>>(entities),
            page,
            pageSize,
            totalItems);
    }

    public virtual async Task<TResponse> GetByIdAsync(int id, bool includeDeleted = false)
    {
        var entity = await repository.GetByIdAsync(id, includeDeleted) ?? throw new NotFoundException($"{EntityName} with ID {id} was not found.");

        return mapper.Map<TResponse>(entity);
    }

    public virtual async Task<TResponse> UpdateAsync(int id, TRequest request)
    {
        var entity = await repository.GetByIdAsync(id, true) ?? throw new NotFoundException($"{EntityName} with ID {id} was not found.");

        mapper.Map(request, entity);

        await repository.UpdateAsync(entity);

        return mapper.Map<TResponse>(entity);
    }

    public virtual async Task DeleteAsync(int id)
    {
        var entity = await repository.GetByIdAsync(id, true) ?? throw new NotFoundException($"{EntityName} with ID {id} was not found.");
        if (entity.IsDeleted)
            throw new BadRequestException($"{EntityName} with ID {id} was deleted.");

        await repository.DeleteAsync(id);
    }

    public virtual async Task<TResponse> RestoreAsync(int id)
    {
        var entity = await repository.GetByIdAsync(id, true) ?? throw new NotFoundException($"{EntityName} with ID {id} was not found.");
        if (!entity.IsDeleted)
            throw new BadRequestException($"{EntityName} with ID {id} was not deleted.");

        var restoredEntity = await repository.RestoreAsync(id);

        return mapper.Map<TResponse>(restoredEntity);
    }
}
