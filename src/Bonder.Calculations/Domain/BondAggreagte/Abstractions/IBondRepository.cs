using Domain.BondAggreagte.Abstractions.Dto.GetPriceSorted;
using Domain.BondAggreagte.ValueObjects;
using Domain.BondAggreagte.ValueObjects.Identities;

namespace Domain.BondAggreagte.Abstractions;

public interface IBondRepository
{
    Task UpdateAsync(Bond bond, CancellationToken token);

    Task<int> CountAsync(CancellationToken token);

    Task<List<Bond>> TakeRangeAsync(Range range, CancellationToken token);

    Task<List<Bond>> GetAllFloatingAsync(CancellationToken token);

    Task RefreshAsync(IEnumerable<Ticker> oldBondTickers, CancellationToken token);

    Task<List<Ticker>> UpdateIncomesAsync(IEnumerable<KeyValuePair<Ticker, StaticIncome>> bonds, CancellationToken token);

    Task<GetPriceSortedResponse> GetPriceSortedAsync(GetPriceSortedRequest filter, CancellationToken token, IEnumerable<Ticker>? tickers = null, IEnumerable<Guid>? uids = null, bool takeAll = false);

    Task AddAsync(IEnumerable<Bond> bonds, CancellationToken token);

    Task<List<Bond>> GetByTickersAsync(IEnumerable<Ticker> tickers, CancellationToken token);
}