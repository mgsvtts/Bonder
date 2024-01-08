using Domain.BondAggreagte;

namespace Domain.BondAggreagte.Repositories;
public interface IBondRepository
{
    Task AddOrUpateAsync(IEnumerable<Bond> bonds, CancellationToken token = default);
}