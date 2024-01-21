using Domain.BondAggreagte;
using Domain.BondAggreagte.ValueObjects;

namespace Application.Calculation.Common.Interfaces;

public interface ITInkoffHttpClient
{
    public Task<IEnumerable<Bond>> GetBondsByTickersAsync(IEnumerable<Ticker> tickers, CancellationToken token = default);
}
