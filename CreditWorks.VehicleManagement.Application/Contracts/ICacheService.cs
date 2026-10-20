using System.Collections.Immutable;

namespace CreditWorks.VehicleManagement.Application.Contracts
{
    public interface ICacheService
    {
        void Set<T>(IEnumerable<T> items, string cacheKey);

        bool TryGet<T>(string cacheKey, out ImmutableArray<T> items);

        void Remove(string cacheKey);
    }
}