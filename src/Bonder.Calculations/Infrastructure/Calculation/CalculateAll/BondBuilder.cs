using Application.Calculation.Common.Abstractions;
using Domain.BondAggreagte;
using Domain.BondAggreagte.ValueObjects;
using Mapster;
using MapsterMapper;

namespace Infrastructure.Calculation.CalculateAll;

public sealed class BondBuilder : IBondBuilder
{
    private readonly ITInkoffHttpClient _tinkoffHttpClient;
    private readonly ITinkoffGrpcClient _tinkoffGrpcClient;
    private readonly IDohodHttpClient _dohodHttpClient;
    private readonly IMoexHttpClient _moexHttpClient;

    public BondBuilder(ITInkoffHttpClient tinkoffHttpClient,
                       ITinkoffGrpcClient tinkoffGrpcClient,
                       IDohodHttpClient dohodHttpClient,
                       IMoexHttpClient moexHttpClient)
    {
        _tinkoffHttpClient = tinkoffHttpClient;
        _tinkoffGrpcClient = tinkoffGrpcClient;
        _dohodHttpClient = dohodHttpClient;
        _moexHttpClient = moexHttpClient;
    }

    public async Task<Bond> BuildAsync(Ticker ticker, CancellationToken token = default)
    {
        var bond = await _tinkoffHttpClient.GetBondByTickerAsync(ticker, token);

        var ratingTask = _dohodHttpClient.GetBondRatingAsync(bond.Identity.Isin, token);

        Task<List<Coupon>> couponTask;
        if (bond.IsAmortized)
        {
            couponTask = _moexHttpClient.GetAmortizedCouponsAsync(bond.Identity.Ticker, token);
        }
        else
        {
            couponTask = _tinkoffGrpcClient.GetCouponsAsync(bond.Identity.InstrumentId, token);
        }

        await Task.WhenAll(ratingTask, couponTask);

        return (bond, couponTask.Result, ratingTask.Result).Adapt<Bond>();
    }

    public async Task<List<Bond>> BuildAsync(IEnumerable<Ticker> tickers, CancellationToken token = default)
    {
        var tasks = tickers.Select(x => BuildAsync(x, token)).ToList();

        await Task.WhenAll(tasks);

        return tasks.Select(x => x.Result).ToList();
    }
}