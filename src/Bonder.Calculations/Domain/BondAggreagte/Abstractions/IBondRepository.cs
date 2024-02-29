using Domain.BondAggreagte.Abstractions.Dto;
using Domain.BondAggreagte.ValueObjects;
using Domain.BondAggreagte.ValueObjects.Identities;

namespace Domain.BondAggreagte.Abstractions;

public interface IBondRepository
{
    Task UpdateAsync(Bond bond, CancellationToken token = default);

    Task<int> CountAsync(CancellationToken token = default);

    Task<List<Bond>> TakeRangeAsync(Range range, CancellationToken token = default);

    Task<List<Bond>> GetAllFloatingAsync(CancellationToken token = default);

    Task RefreshAsync(IEnumerable<Ticker> oldBondTickers, CancellationToken token = default);

    Task<List<Ticker>> UpdateIncomesAsync(IEnumerable<KeyValuePair<Ticker, StaticIncome>> bonds, CancellationToken token = default);

    Task<GetPriceSortedResponse> GetPriceSortedAsync(GetPriceSortedRequest filter, IEnumerable<Ticker>? tickers = null, IEnumerable<Guid>? uids = null, bool takeAll = false, CancellationToken token = default);

    Task AddAsync(IEnumerable<Bond> bonds, CancellationToken token = default);

    Task<List<Bond>> GetByTickersAsync(IEnumerable<Ticker> tickers, CancellationToken token = default);
}