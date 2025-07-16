using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonalFinanceTracker_EnterpriseEdition.Application.Abstractions;

public interface ICacheService
{
    Task CreateCacheAsync(string key, object information);
    Task<T> GetCacheAsync<T>(string key);
}
