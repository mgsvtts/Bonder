using Domain.BondAggreagte.ValueObjects;

namespace Domain.BondAggreagte.Repositories;

public interface IBondRepository
{
    Task UpdateAsync(IEnumerable<KeyValuePair<Ticker, StaticIncome>> bonds, CancellationToken token = default);

    Task<List<Bond>> GetPriceSortedBondsAsync(CancellationToken token = default);
}
