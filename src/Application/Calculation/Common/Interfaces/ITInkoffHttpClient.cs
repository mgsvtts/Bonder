using Domain.BondAggreagte;
using Domain.BondAggreagte.ValueObjects;

namespace Application.Calculation.Common.Interfaces;

public interface ITInkoffHttpClient
{
    public Task<Dictionary<Ticker, StaticIncome>> GetBondPriceAsync(IEnumerable<Ticker> tickers, CancellationToken token = default);
    public Task<IEnumerable<Bond>> GetBondsByTickersAsync(IEnumerable<Ticker> tickers, CancellationToken token = default);
}
