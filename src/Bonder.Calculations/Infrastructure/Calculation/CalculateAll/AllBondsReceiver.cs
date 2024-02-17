using Application.Calculation.Common.Interfaces;
using Domain.BondAggreagte.Abstractions;
using Domain.BondAggreagte.ValueObjects;
using Tinkoff.InvestApi;

namespace Infrastructure.Calculation.CalculateAll;

public class AllBondsReceiver : IAllBondsReceiver
{
    private static int _maxRange;
    private readonly ITInkoffHttpClient _tinkoffHttpClient;
    private readonly InvestApiClient _tinkoffApiClient;
    private readonly IBondRepository _bondRepository;

    private static IEnumerable<Ticker>? _cache;

    public AllBondsReceiver(InvestApiClient tinkoffApiClient, ITInkoffHttpClient tinkoffHttpClient, IBondRepository bondRepository)
    {
        _tinkoffApiClient = tinkoffApiClient;
        _tinkoffHttpClient = tinkoffHttpClient;
        _bondRepository = bondRepository;
    }

    public async Task<IEnumerable<KeyValuePair<Ticker, StaticIncome>>> ReceiveAsync(Range takeRange, CancellationToken token)
    {
        var response = await GetFromCacheAsync(takeRange, token);

        var bonds = await _tinkoffHttpClient.GetBondPriceAsync(response, token);

        return bonds.Where(x => x.Value.AbsolutePrice != 0);
    }

    public int GetMaxRange()
    {
        return _maxRange;
    }

    private async Task<IEnumerable<Ticker>> GetFromCacheAsync(Range range, CancellationToken token)
    {
        if (range.Start.Value > GetMaxRange())
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
        _maxRange = instruments.Count;

        _cache = instruments.Select(x => new Ticker(x.Ticker));

        await _bondRepository.RefreshAsync(_cache, token);
    }
}