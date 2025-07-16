namespace PersonalFinanceTracker_EnterpriseEdition.Application.Abstractions;

public interface ICacheService
{
    Task CreateCacheAsync(string key, object information);
    Task<T> GetCacheAsync<T>(string key);
    Task DeleteAsync(string key);
}
