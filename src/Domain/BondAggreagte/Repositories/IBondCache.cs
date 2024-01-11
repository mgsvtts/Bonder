using Domain.BondAggreagte;
using Domain.BondAggreagte.ValueObjects;

namespace Domain.BondAggreagte.Repositories;
public interface IBondCache
{
    Task AddOrUpdateAsync(IEnumerable<Bond> bonds, CancellationToken token = default);
    Task<Bond?> GetByIdAsync(BondId id, CancellationToken token = default);
}