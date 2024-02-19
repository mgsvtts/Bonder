using Domain.BondAggreagte;
using Domain.BondAggreagte.ValueObjects;

namespace Application.Calculation.Common.Abstractions;

public interface IBondBuilder
{
    public Task<List<Bond>> BuildAsync(IEnumerable<Ticker> tickers, CancellationToken token = default);

    public Task<Bond> BuildAsync(Ticker ticker, CancellationToken token = default);
}