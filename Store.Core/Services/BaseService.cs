using System;
using AutoMapper;
using Store.Core.DTOs;
using Store.Core.Entities;
using Store.Core.Exceptions;
using Store.Core.Interfaces.Repositories;
using Store.Core.Interfaces.Services;

namespace Store.Core.Services;

public abstract class BaseService<TEntity, TResponse, TRequest, TFilter>(IBaseRepository<TEntity, TFilter> repository, IMapper mapper)
    : IBaseService<TResponse, TRequest, TFilter>
        where TEntity : BaseEntity<int>
        where TFilter : IFilter
{
    protected const int MaxPageSize = 100;
    protected IMapper _mapper => mapper;
    protected virtual string EntityName => typeof(TEntity).Name;

    public virtual async Task<TResponse> CreateAsync(TRequest request)
    {
        var entity = _mapper.Map<TEntity>(request);
        await repository.AddAsync(entity);
        return _mapper.Map<TResponse>(entity);
    }

    public virtual Task<PagedResult<TResponse>> GetAllPagedAsync(TFilter filter, int page, int pageSize) =>
        PageingAsync(page, pageSize, () => repository.CountAsync(filter), (skip, take) => repository.GetPageAsync(filter, skip, take));

    public virtual async Task<TResponse> GetByIdAsync(int id, bool includeDeleted = false) =>
         _mapper.Map<TResponse>(await repository.GetByIdAsync(id, includeDeleted)
            ?? throw new NotFoundException($"{EntityName} with ID {id} was not found."));


    public virtual async Task<TResponse> UpdateAsync(int id, TRequest request)
    {
        var entity = await repository.GetByIdAsync(id)
            ?? throw new NotFoundException($"{EntityName} with ID {id} was not found.");

        _mapper.Map(request, entity);
        await repository.UpdateAsync(entity);

        return _mapper.Map<TResponse>(entity);
    }

    public virtual async Task DeleteAsync(int id)
    {
        if (!await repository.ExistsByIdAsync(id))
            throw new NotFoundException($"{EntityName} with ID {id} was not found.");

        await repository.DeleteAsync(id);
    }

    public virtual async Task<TResponse> RestoreAsync(int id)
    {
        // Check here so a missing or already-active row is a 404/409, not a 500 from the repository.
        var existing = await repository.GetByIdAsync(id, includeDeleted: true)
            ?? throw new NotFoundException($"{EntityName} with ID {id} was not found.");

        if (!existing.IsDeleted)
            throw new ConflictException($"{EntityName} with ID {id} is not deleted.");

        var restored = await repository.RestoreAsync(id);
        return _mapper.Map<TResponse>(restored);
    }

    /// <summary>
    /// Validates paging input, runs the count and page queries, and maps the page to response DTOs.
    /// Both delegates must use the same criteria so the total matches the pages.
    /// </summary>
    protected async Task<PagedResult<TResponse>> PageingAsync(int page, int pageSize, Func<Task<int>> countAsync, Func<int, int, Task<IReadOnlyList<TEntity>>> fetchPageAsync)
    {
        ValidatePaging(page, pageSize);

        var skip = (int)Math.Min((long)(page - 1) * pageSize, int.MaxValue);
        var totalItems = await countAsync();

        if (totalItems == 0 || skip >= totalItems)
            return new PagedResult<TResponse>([], page, pageSize, totalItems);

        var entities = await fetchPageAsync(skip, pageSize);

        return new PagedResult<TResponse>(
            _mapper.Map<IReadOnlyList<TResponse>>(entities), page, pageSize, totalItems);
    }

    protected static void ValidatePaging(int page, int pageSize)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(page, 1);
        ArgumentOutOfRangeException.ThrowIfLessThan(pageSize, 1);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(pageSize, MaxPageSize);
    }
}