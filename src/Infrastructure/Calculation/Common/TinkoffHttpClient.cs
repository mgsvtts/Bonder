using Application.Calculation.Common.Interfaces;
using Domain.BondAggreagte;
using Domain.BondAggreagte.ValueObjects;
using Infrastructure.Calculation.Dto.GetBonds;
using Infrastructure.Calculation.Dto.GetBonds.TInkoffApiData;
using MapsterMapper;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace Infrastructure.Calculation.Common;

public class TinkoffHttpClient : ITInkoffHttpClient
{
    private readonly HttpClient _client;
    private readonly ITinkoffGrpcClient _grpcClient;
    private readonly IDohodHttpClient _dohodHttpClient;
    private readonly string _tinkoffUrl;
    private readonly IMapper _mapper;

    public TinkoffHttpClient(HttpClient client,
                             ITinkoffGrpcClient grpcClient,
                             IMapper mapper,
                             IDohodHttpClient dohodHttpClient,
                             string token,
                             string tinkoffUrl)
    {
        _client = client;
        _tinkoffUrl = tinkoffUrl;
        _mapper = mapper;
        _grpcClient = grpcClient;
        _dohodHttpClient = dohodHttpClient;

        _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
    }

    public async Task<IEnumerable<Bond>> GetBondsByTickersAsync(IEnumerable<Ticker> tickers, CancellationToken token = default)
    {
        var request = SerializeToRequest(tickers);

        var content = new HttpRequestMessage
        {
            Content = new StringContent(request, Encoding.UTF8, "application/json"),
            RequestUri = new Uri(_tinkoffUrl + "/api/trading/bonds/list"),
            Method = HttpMethod.Post
        };

        var response = await _client.SendAsync(content, token);

        response.EnsureSuccessStatusCode();

        var serializedResponse = await response.Content.ReadFromJsonAsync<TinkoffResponse>(cancellationToken: token)
                                 ?? throw new InvalidOperationException("Ошибка получения ответа от Tinkoff");

        return await GetBondsParallelAsync(serializedResponse, token);
    }

    private async Task<IEnumerable<Bond>> GetBondsParallelAsync(TinkoffResponse serializedResponse, CancellationToken token = default)
    {
        var tasks = new List<Task<Bond>>();

        foreach (var bond in serializedResponse.Payload.Values)
        {
            tasks.Add(ConvertToDomainBondAsync(bond, token));
        }

        await Task.WhenAll(tasks);

        return tasks.Select(x => x.Result);
    }

    private async Task<Bond> ConvertToDomainBondAsync(TinkoffValue value, CancellationToken token = default)
    {
        var coupons = _grpcClient.GetBondCouponsAsync(value.Symbol.SecurityUids.InstrumentUid, token);

        var rating = _dohodHttpClient.GetBondRatingAsync(new Isin(value.Symbol.Isin), token);

        await Task.WhenAll(coupons, rating);

        return _mapper.Map<Bond>((value, coupons.Result, rating.Result));
    }

    private static string SerializeToRequest(IEnumerable<Ticker> tickers)
    {
        var request = new GetBondsByTickersRequest
        {
            Tickers = tickers.Select(x => x.Value)
        };

        return JsonSerializer.Serialize(request);
    }
}