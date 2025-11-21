using GestaoUpc.Domain.Data.Contexts;
using GestaoUpc.Domain.Data.Extensions;
using GestaoUpc.Domain.Entities;
using GestaoUpc.Domain.Repositories;
using GestaoUpc.Domain.DTOs.Requests.PagedRequest;
using GestaoUpc.Domain.DTOs.Responses;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace GestaoUpc.Domain.Data.Repositories;

public class RepositoryBase<T> : IRepositoryBase<T> where T : BaseEntity
{
    protected readonly DbSet<T> _dbSet;
    protected readonly GestaoUpcDbContext _context;

    public RepositoryBase(GestaoUpcDbContext db)
    {
        _context = db;
        _dbSet = db.Set<T>();
    }

    public async Task<T> AddAsync(T entity, bool isActive = true)
    {
        entity.Active = isActive;
        entity.CreatedAt = DateTime.UtcNow;
        entity.UpdatedAt = null;
        entity.UpdatedBy = null;

        await _context.AddAsync(entity);
        await _context.SaveChangesAsync();

        return entity;
    }

    public async Task<List<T>> AddRangeAsync(List<T> entities)
    {
        foreach (var entity in entities)
        {
            entity.Active = true;
            entity.CreatedAt = DateTime.UtcNow;
            entity.UpdatedAt = null;
            entity.UpdatedBy = null;
            await _context.Set<T>().AddAsync(entity);
        }

        await _context.SaveChangesAsync();

        return entities;
    }

    public async Task UpdateRangeAsync(List<T> entities)
    {
        foreach (var item in entities)
        {
            item.UpdatedAt = DateTime.UtcNow;
            _context.Entry(item).State = EntityState.Modified;
        }

        await _context.SaveChangesAsync();
    }

    public async Task<T?> GetByIdAsync(Guid id)
    {
        var find = await _dbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.NavigationId == id && x.Active);

        return find;
    }

    public async Task<List<T>> GetAllAsync() => await _dbSet.AsNoTracking().Where(x => x.Active).ToListAsync();

    public async Task<T> UpdateAsync(T entity)
    {
        entity.UpdatedAt = DateTime.UtcNow;
        _context.Entry(entity).State = EntityState.Modified;
        await _context.SaveChangesAsync();

        return entity;
    }

    public async Task<TEntity> UpdateDataAsync<TEntity>(TEntity entity,
    params Expression<Func<TEntity, IEnumerable<object>>>[] collections) where TEntity : class
    {
        // Força o detach da entidade se ela estiver sendo rastreada
        var entry = _context.Entry(entity);
        if (entry.State != EntityState.Detached)
        {
            entry.State = EntityState.Detached;
        }

        // Anexa a entidade novamente
        _context.Set<TEntity>().Attach(entity);
        entry.State = EntityState.Modified;
        foreach (var collectionExpr in collections)
        {
            var collection = collectionExpr.Compile()(entity);

            if (collection == null)
                continue;

            foreach (var item in collection)
            {
                var itemEntry = _context.Entry(item);
                var idProp = item.GetType().GetProperty("Id");
                var idValue = idProp != null ? (int)idProp.GetValue(item)! : 0;

                if (idValue == 0)
                    itemEntry.State = EntityState.Added;
                else
                    itemEntry.State = EntityState.Modified;
            }
        }

        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task DeleteAsync(T entity)
    {
        entity.Active = false;
        entity.UpdatedAt = DateTime.UtcNow;
        _context.Entry(entity).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    public async Task DeleteRangeAsync(List<T> entities)
    {
        foreach (var item in entities)
        {
            item.Active = false;
            item.UpdatedAt = DateTime.UtcNow;
            _context.Entry(item).State = EntityState.Modified;
        }
        await _context.SaveChangesAsync();
    }

    public async Task<List<T>> GetAllAsync(Expression<Func<T, bool>> predicate)
    {
        return await _dbSet.Where(predicate).ToListAsync();
    }

    public async Task DeleteRangeAsync(List<Guid> navigationIds)
    {
        var list = await GetAllAsync(x => navigationIds.Contains(x.NavigationId));
        await DeleteRangeAsync(list);
    }

    public async Task<DynamicQueryResult<T>> GetPagedAsync(DynamicQuery query)
    {
        var queryable = _dbSet.AsNoTracking().Where(x => x.Active).AsQueryable();
        
        // Usa a extensão ToPagedAsync que já está implementada
        return await queryable.ToPagedAsync(query);
    }
}