using Application.Calculation.Common.Interfaces;
using Domain.BondAggreagte.ValueObjects;
using System;
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

    public AllBondsReceiver(InvestApiClient tinkoffApiClient, ITInkoffHttpClient tinkoffHttpClient)
    {
        _tinkoffApiClient = tinkoffApiClient;
        _tinkoffHttpClient = tinkoffHttpClient;
    }

    public async Task<IEnumerable<Domain.BondAggreagte.Bond>> ReceiveAsync(CancellationToken token)
    {
        var response = await _tinkoffApiClient.Instruments.BondsAsync(token);

        var bonds = await _tinkoffHttpClient.GetBondsByTickersAsync(response.Instruments.Select(x => new Ticker(x.Ticker)), token);

        return bonds;
    }

}
