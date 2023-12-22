using Application.Calculation.CalculateTickers.Interfaces;
using Application.Calculation.Common.Exceptions;
using Google.Protobuf.WellKnownTypes;
using MapsterMapper;
using Tinkoff.InvestApi;
using Tinkoff.InvestApi.V1;

namespace Infrastructure.Calculation;

public class TinkoffGrpcClient : ITinkoffGrpcClient
{
    private readonly InvestApiClient _tinkoffApiClient;
    private readonly ITInkoffHttpClient _tinkoffHttpClient;
    private readonly IMapper _mapper;

    public TinkoffGrpcClient(InvestApiClient client, IMapper mapper, ITInkoffHttpClient tinkoffHttpClient)
    {
        _tinkoffApiClient = client;
        _mapper = mapper;
        _tinkoffHttpClient = tinkoffHttpClient;
    }

    public async Task<Domain.BondAggreagte.Bond> GetBondByTickerAsync(string ticker, CancellationToken token = default)
    {
        var bonds = await _tinkoffApiClient.Instruments.BondsAsync(token);

        return await GetBondAsync(bonds, ticker, token);
    }

    public async Task<List<Domain.BondAggreagte.Bond>> GetBondsByTickersAsync(IEnumerable<string> tickers, CancellationToken token = default)
    {
        var bonds = await _tinkoffApiClient.Instruments.BondsAsync(token);

        var tasks = new List<Task<Domain.BondAggreagte.Bond>>();

        foreach (var ticker in tickers)
        {
            tasks.Add(GetBondAsync(bonds, ticker, token));
        }

        await Task.WhenAll(tasks);

        return tasks.Select(x => x.Result).ToList();
    }

    private async Task<Domain.BondAggreagte.Bond> GetBondAsync(BondsResponse bonds, string ticker, CancellationToken token)
    {
        var bond = bonds.Instruments.FirstOrDefault(x => x.Ticker.ToUpper() == ticker.ToUpper())
                   ?? throw new BondNotFoundException(ticker);

        var couponsTask = _tinkoffApiClient.Instruments.GetBondCouponsAsync(new GetBondCouponsRequest
        {
            InstrumentId = bond.Uid,
            From = Timestamp.FromDateTime(DateTime.MinValue.ToUniversalTime()),
            To = Timestamp.FromDateTime(DateTime.MaxValue.ToUniversalTime())
        }, cancellationToken: token);

        var priceTask = _tinkoffHttpClient.GetBondPriceAsync(bond.Uid, token);

        await Task.WhenAll(couponsTask.ResponseAsync, priceTask);

        return _mapper.Map<Domain.BondAggreagte.Bond>((bond, couponsTask.ResponseAsync.Result, priceTask.Result));
    }
}