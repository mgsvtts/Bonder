using Application.Calculation.Common.Exceptions;
using Application.Calculation.Common.Interfaces;
using Domain.BondAggreagte.ValueObjects;
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

    public async Task<Domain.BondAggreagte.Bond> GetBondByTickerAsync(Ticker ticker, CancellationToken token = default)
    {
        var bonds = await _tinkoffApiClient.Instruments.BondsAsync(token);

        return await GetBondByTickerAsync(bonds, ticker, token);
    }

    public async Task<List<Domain.BondAggreagte.Bond>> GetBondsByTickersAsync(IEnumerable<Ticker> tickers, CancellationToken token = default)
    {
        var bonds = await _tinkoffApiClient.Instruments.BondsAsync(token);

        var tasks = new List<Task<Domain.BondAggreagte.Bond>>();

        foreach (var ticker in tickers)
        {
            tasks.Add(GetBondByTickerAsync(bonds, ticker, token));
        }

        await Task.WhenAll(tasks);

        return tasks.Select(x => x.Result).ToList();
    }

    public async Task<List<Domain.BondAggreagte.Bond>> GetBondsByUidsAsync(IEnumerable<Guid> uids, CancellationToken token = default)
    {
        var tasks = new List<Task<Domain.BondAggreagte.Bond>>();

        foreach (var uid in uids)
        {
            tasks.Add(GetBondByUidAsync(uid, token));
        }

        await Task.WhenAll(tasks);

        return tasks.Select(x => x.Result).ToList();
    }

    public async Task<Domain.BondAggreagte.Bond> GetBondByUidAsync(Guid uid, CancellationToken token = default)
    {
        var bond = await _tinkoffApiClient.Instruments.BondByAsync(new InstrumentRequest
        {
            IdType = InstrumentIdType.Uid,
            Id = uid.ToString()
        }, cancellationToken: token);

        return await ConvertToDomainBondAsync(bond.Instrument, token);
    }

    public async Task<Domain.BondAggreagte.Bond> GetBondByFigiAsync(Figi figi, CancellationToken token = default)
    {
        var bond = await _tinkoffApiClient.Instruments.BondByAsync(new InstrumentRequest
        {
            IdType = InstrumentIdType.Figi,
            Id = figi.Value.ToString()
        }, cancellationToken: token);

        return await ConvertToDomainBondAsync(bond.Instrument, token);
    }

    public async Task<List<Domain.BondAggreagte.Bond>> GetBondsByFigisAsync(IEnumerable<Figi> figis, CancellationToken token = default)
    {
        var tasks = new List<Task<Domain.BondAggreagte.Bond>>();

        foreach (var figi in figis)
        {
            tasks.Add(GetBondByFigiAsync(figi, token));
        }

        await Task.WhenAll(tasks);

        return tasks.Select(x => x.Result).ToList();
    }

    private async Task<Domain.BondAggreagte.Bond> GetBondByTickerAsync(BondsResponse bonds, Ticker ticker, CancellationToken token)
    {
        var bond = bonds.Instruments.FirstOrDefault(x => x.Ticker.ToUpper() == ticker.Value.ToUpper())
                   ?? throw new BondNotFoundException(ticker.Value);

        return await ConvertToDomainBondAsync(bond, token);
    }

    private async Task<Domain.BondAggreagte.Bond> ConvertToDomainBondAsync(Tinkoff.InvestApi.V1.Bond bond, CancellationToken token)
    {
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