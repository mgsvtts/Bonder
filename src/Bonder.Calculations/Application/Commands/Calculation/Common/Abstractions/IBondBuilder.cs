using Domain.BondAggreagte;
using Domain.BondAggreagte.ValueObjects.Identities;

namespace Application.Commands.Calculation.Common.Abstractions;

public interface IBondBuilder
{
    public Task<IEnumerable<Bond>> BuildAsync(IEnumerable<Ticker> tickers, CancellationToken token = default);

    public Task<Bond> BuildAsync(Ticker ticker, CancellationToken token = default);
}