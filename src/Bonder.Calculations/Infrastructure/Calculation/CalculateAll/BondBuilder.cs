using Domain.BondAggreagte;
using Domain.BondAggreagte.Abstractions;
using Domain.BondAggreagte.Abstractions.Dto.Moex;
using Domain.BondAggreagte.ValueObjects.Identities;
using Domain.BondAggreagte.ValueObjects.Incomes;
using Mapster;

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

    public async Task<Bond> BuildAsync(Ticker ticker, CancellationToken token)
    {
        var bondResponse = await _tinkoffHttpClient.GetBondByTickerAsync(ticker, token);

        var ratingTask = _dohodHttpClient.GetBondRatingAsync(bondResponse.BondId.Isin, token);

        Task<List<Coupon>?> couponTask = Task.FromResult<List<Coupon>?>(null);
        Task<MoexResponse> moexResponse = Task.FromResult<MoexResponse>(default);
        if (bondResponse.IsAmortized)
        {
            moexResponse = _moexHttpClient.GetMoexResponseAsync(bondResponse.BondId.Ticker, token);
        }
        else
        {
            couponTask = _tinkoffGrpcClient.GetCouponsAsync(bondResponse.BondId.InstrumentId, token);
        }

        await Task.WhenAll(ratingTask, couponTask, moexResponse);

        return (bondResponse, couponTask.Result, ratingTask.Result, moexResponse.Result).Adapt<Bond>();
    }

    public async Task<IEnumerable<Bond>> BuildAsync(IEnumerable<Ticker> tickers, CancellationToken token)
    {
        var tasks = tickers.Select(x => BuildAsync(x, token)).ToList();

        await Task.WhenAll(tasks);

        return tasks.Select(x => x.Result);
    }
}