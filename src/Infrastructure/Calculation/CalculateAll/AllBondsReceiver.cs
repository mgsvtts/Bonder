using Application.Calculation.Common.Interfaces;
using Domain.BondAggreagte.ValueObjects;
using Tinkoff.InvestApi;

namespace Infrastructure.Calculation.CalculateAll;

public class AllBondsReceiver : IAllBondsReceiver
{
    private readonly ITInkoffHttpClient _tinkoffHttpClient;
    private readonly InvestApiClient _tinkoffApiClient;

    private IEnumerable<string>? _cache;

    public int MaxRange { get; private set; }

    public AllBondsReceiver(InvestApiClient tinkoffApiClient, ITInkoffHttpClient tinkoffHttpClient)
    {
        _tinkoffApiClient = tinkoffApiClient;
        _tinkoffHttpClient = tinkoffHttpClient;

        MaxRange = 0;
    }

    public async Task<IEnumerable<Domain.BondAggreagte.Bond>> ReceiveAsync(Range takeRange, CancellationToken token)
    {
        var response = await GetFromCacheAsync(takeRange, token);

        var bonds = await _tinkoffHttpClient.GetBondsByTickersAsync(response.Select(x => new Ticker(x)), token);

        return bonds.Where(x => x.Percents.PricePercent != 0);
    }

    private async Task<IEnumerable<string>> GetFromCacheAsync(Range range, CancellationToken token)
    {
        if (range.Start.Value > MaxRange)
        {
            _cache = null;
        }

        if (_cache == null)
        {
            await InitCacheAsync(token);
        }

        return _cache.Take(range);
    }

    private async Task InitCacheAsync(CancellationToken token)
    {
        var instruments = (await _tinkoffApiClient.Instruments.BondsAsync(token)).Instruments;

        MaxRange = instruments.Count;

        _cache = instruments.Select(x => x.Ticker);
    }
}
