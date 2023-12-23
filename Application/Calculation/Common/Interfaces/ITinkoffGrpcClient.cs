using Domain.BondAggreagte;
using Domain.BondAggreagte.ValueObjects;

namespace Application.Calculation.Common.Interfaces;

public interface ITinkoffGrpcClient
{
    public Task<Bond> GetBondByTickerAsync(Ticker ticker, CancellationToken token = default);

    public Task<List<Bond>> GetBondsByTickersAsync(IEnumerable<Ticker> tickers, CancellationToken token = default);

    public Task<List<Bond>> GetBondsByUidsAsync(IEnumerable<Guid> uids, CancellationToken token = default);

    public Task<Bond> GetBondByUidAsync(Guid uid, CancellationToken token = default);

    Task<List<Bond>> GetBondsByFigisAsync(IEnumerable<Figi> figis, CancellationToken token = default);

    Task<Bond> GetBondByFigiAsync(Figi figi, CancellationToken token = default);
}