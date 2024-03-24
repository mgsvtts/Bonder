using Domain.BondAggreagte.ValueObjects.Identities;

namespace Domain.BondAggreagte.Abstractions;

public interface IBondBuilder
{
    public Task<IEnumerable<Bond>> BuildAsync(IEnumerable<Ticker> tickers, CancellationToken token);

    public Task<Bond> BuildAsync(Ticker ticker, CancellationToken token);
}