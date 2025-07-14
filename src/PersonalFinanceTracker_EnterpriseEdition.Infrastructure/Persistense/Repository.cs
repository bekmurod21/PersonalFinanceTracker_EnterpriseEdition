using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using PersonalFinanceTracker_EnterpriseEdition.Application.Abstractions;
using PersonalFinanceTracker_EnterpriseEdition.Domain.Commons;

namespace PersonalFinanceTracker_EnterpriseEdition.Infrastructure.Persistense;

public class Repository<T>(ApplicationDbContext dbContext) : IRepository<T> where T : Auditable
{
    private readonly ApplicationDbContext _dbContext = dbContext;
    private readonly DbSet<T> _dbSet = dbContext.Set<T>();

    public async Task<T?> GetByIdAsync(Guid id)
        => await _dbSet.FirstOrDefaultAsync(e => e.Id == id && !e.IsDeleted);

    public async Task<List<T>> GetAllAsync(Expression<Func<T, bool>>? predicate = null)
    {
        var query = _dbSet.AsQueryable().Where(e => !e.IsDeleted);
        if (predicate != null)
            query = query.Where(predicate);
        return await query.ToListAsync();
    }

    public IQueryable<T> Query(Expression<Func<T, bool>>? predicate = null)
    {
        var query = _dbSet.AsQueryable().Where(e => !e.IsDeleted);
        if (predicate != null)
            query = query.Where(predicate);
        return query;
    }

    public async Task<T> AddAsync(T entity)
    {
        var entry = await _dbSet.AddAsync(entity);
        return entry.Entity;
    }

    public async Task<T> UpdateAsync(T entity)
    {
        _dbSet.Update(entity);
        return entity;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var entity = await _dbSet.FirstOrDefaultAsync(e => e.Id == id && !e.IsDeleted);
        if (entity == null) return false;
        entity.IsDeleted = true;
        return true;
    }

    public async Task<int> SaveChangesAsync()
        => await _dbContext.SaveChangesAsync();
} 