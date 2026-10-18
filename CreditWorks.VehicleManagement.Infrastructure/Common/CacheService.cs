using CreditWorks.VehicleManagement.Application.Contracts;
using Microsoft.Extensions.Caching.Memory;
using System.Collections.Immutable;

namespace CreditWorks.VehicleManagement.Infrastructure.Common
{
    public class CacheService(IMemoryCache cache) : ICacheService
    {        
        public void Set<T>(IEnumerable<T> items, string cacheKey)
            => cache.Set(cacheKey, items.ToImmutableArray());

        public bool TryGet<T>(string cacheKey, out ImmutableArray<T> items)
        {
            if (cache.TryGetValue(cacheKey, out object? list)
                && list is ImmutableArray<T> cached)
            {
                items = cached;
                return true;
            }
            items = [];
            return false;
        }

        public void Remove(string cacheKey)
        {
            cache.Remove(cacheKey);
        }
    }
}
