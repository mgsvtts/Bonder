using Domain.BondAggreagte;

namespace Application.Calculation.CalculateTickers.Interfaces;

public interface ITinkoffGrpcClient
{
    public Task<Bond> GetBondByTickerAsync(string ticker, CancellationToken token = default);

    public Task<List<Bond>> GetBondsByTickersAsync(IEnumerable<string> tickers, CancellationToken token = default);
}