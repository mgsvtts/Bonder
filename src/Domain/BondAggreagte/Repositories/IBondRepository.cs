using Domain.BondAggreagte;

namespace Domain.BondAggreagte.Repositories;
public interface IBondRepository
{
    Task AddOrUpateAsync(IEnumerable<Bond> bonds, CancellationToken token = default);
    Task<List<Bond>> GetPriceSortedBondsAsync(CancellationToken token = default);
    Task<List<Bond>> GetRatingSortedBondsAsync(CancellationToken token = default);
}