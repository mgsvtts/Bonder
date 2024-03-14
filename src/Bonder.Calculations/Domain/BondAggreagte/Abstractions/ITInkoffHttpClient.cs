using Domain.BondAggreagte.Abstractions.Dto.GetPriceSorted;
using Domain.BondAggreagte.ValueObjects;
using Domain.BondAggreagte.ValueObjects.Identities;

namespace Domain.BondAggreagte.Abstractions;

public interface ITInkoffHttpClient
{
    public Task<Dictionary<Ticker, StaticIncome>> GetBondPriceAsync(IEnumerable<Ticker> tickers, CancellationToken token = default);

    public Task<GetBondResponse> GetBondByTickerAsync(Ticker ticker, CancellationToken token = default);
}