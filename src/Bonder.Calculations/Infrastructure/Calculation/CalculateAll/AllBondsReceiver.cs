using Application.Calculation.Common.Abstractions;
using Domain.BondAggreagte.Abstractions;
using Domain.BondAggreagte.ValueObjects;
using Domain.BondAggreagte.ValueObjects.Identities;
using Tinkoff.InvestApi;

namespace Infrastructure.Calculation.CalculateAll;

public sealed class AllBondsReceiver : IAllBondsReceiver
{
    private readonly ITInkoffHttpClient _tinkoffHttpClient;
    private readonly InvestApiClient _tinkoffApiClient;
    private readonly IBondRepository _bondRepository;


    public AllBondsReceiver(InvestApiClient tinkoffApiClient, ITInkoffHttpClient tinkoffHttpClient, IBondRepository bondRepository)
    {
        _tinkoffApiClient = tinkoffApiClient;
        _tinkoffHttpClient = tinkoffHttpClient;
        _bondRepository = bondRepository;
    }

    public async Task<IEnumerable<KeyValuePair<Ticker, StaticIncome>>> ReceiveAsync(CancellationToken token)
    {
        var bonds = await _tinkoffHttpClient.GetBondPriceAsync(await GetAllTickersAsync(token), token);

        return bonds.Where(x => x.Value.AbsolutePrice != 0);
    }

    private async Task<IEnumerable<Ticker>> GetAllTickersAsync(CancellationToken token)
    {
        var tickers = (await _tinkoffApiClient.Instruments.BondsAsync(token))
        .Instruments
        .Select(x => new Ticker(x.Ticker));

        await _bondRepository.RefreshAsync(tickers, token);

        return tickers;
    }
}