using System.Linq.Expressions;
using PersonalFinanceTracker_EnterpriseEdition.Domain.Commons;

namespace PersonalFinanceTracker_EnterpriseEdition.Application.Abstractions;

public interface IRepository<T> where T : Auditable
{
    Task<T> GetByIdAsync(Guid id, string[] includes = null);
    IQueryable<T> Query(Expression<Func<T, bool>> predicate = null, string[] includes = null);
    Task<T> AddAsync(T entity);
    Task<T> UpdateAsync(T entity);
    Task<bool> DeleteAsync(Guid id);
    Task<int> SaveChangesAsync();
} 