using Application.Calculation.Common.Interfaces;
using Domain.BondAggreagte.ValueObjects;
using Google.Protobuf.Collections;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tinkoff.InvestApi;

namespace Infrastructure.Calculation;
public class AllBondsReceiver : IAllBondsReceiver
{
    private readonly ITInkoffHttpClient _tinkoffHttpClient;
    private readonly InvestApiClient _tinkoffApiClient;

    private IEnumerable<Tinkoff.InvestApi.V1.Bond>? _cache;
    private int _maxRange;

    public AllBondsReceiver(InvestApiClient tinkoffApiClient, ITInkoffHttpClient tinkoffHttpClient)
    {
        _tinkoffApiClient = tinkoffApiClient;
        _tinkoffHttpClient = tinkoffHttpClient;

        _maxRange = 0;
    }

    public async Task<IEnumerable<Domain.BondAggreagte.Bond>> ReceiveAsync(Range takeRange, CancellationToken token)
    {
        var response = await GetFromCacheAsync(takeRange, token);
        
        var bonds = await _tinkoffHttpClient.GetBondsByTickersAsync(response.Select(x => new Ticker(x.Ticker)), token);

        return bonds;
    }

    private async Task<IEnumerable<Tinkoff.InvestApi.V1.Bond>> GetFromCacheAsync(Range range, CancellationToken token)
    {
        if(range.Start.Value > _maxRange)
        {
            _cache = null;
        }

        if(_cache == null)
        {
            await InitCacheAsync(token);
        }

        return _cache.Take(range);
    }

    private async Task InitCacheAsync(CancellationToken token)
    {
        var instruments = (await _tinkoffApiClient.Instruments.BondsAsync(token)).Instruments;

        _maxRange = instruments.Count;
        _cache = instruments.OrderByDescending(x => x.Nominal.Units)
                            .ThenBy(x => x.Ticker);
    }
}
