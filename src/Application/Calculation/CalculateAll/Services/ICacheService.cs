using Domain.BondAggreagte;
using Domain.BondAggreagte.ValueObjects;

namespace Infrastructure.Calculation.CalculateAll;
public interface ICacheService
{
    Task CacheAsync(IEnumerable<Bond> bonds, CancellationToken token = default);
}