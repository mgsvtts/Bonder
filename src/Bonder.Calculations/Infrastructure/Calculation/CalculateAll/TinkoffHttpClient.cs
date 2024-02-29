using Application.Calculation.Common.Abstractions;
using Application.Calculation.Common.Abstractions.Dto;
using Domain.BondAggreagte.ValueObjects;
using Domain.BondAggreagte.ValueObjects.Identities;
using Infrastructure.Calculation.Dto.GetBonds;
using Infrastructure.Calculation.Dto.GetBonds.TInkoffApiData;
using Mapster;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace Infrastructure.Calculation.CalculateAll;

public sealed class TinkoffHttpClient : ITInkoffHttpClient
{
    private readonly HttpClient _client;
    private readonly string _tinkoffUrl;

    public TinkoffHttpClient(HttpClient client,
                             string token,
                             string tinkoffUrl)
    {
        _client = client;
        _tinkoffUrl = tinkoffUrl;

        _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
    }

    public async Task<GetBondResponse> GetBondByTickerAsync(Ticker ticker, CancellationToken token = default)
    {
        var response = await GetTinkoffResponseAsync([ticker], token);

        return response.Payload.Values.First().Adapt<GetBondResponse>();
    }

    public async Task<Dictionary<Ticker, StaticIncome>> GetBondPriceAsync(IEnumerable<Ticker> tickers, CancellationToken token = default)
    {
        var result = new Dictionary<Ticker, StaticIncome>();

        var response = await GetTinkoffResponseAsync(tickers, token);

        foreach (var bond in response.Payload.Values)
        {
            result.Add(new Ticker(bond.Symbol.Ticker), StaticIncome.FromAbsoluteValues(bond.Price?.Value ?? 0, bond.Nominal));
        }

        return result;
    }

    private async Task<TinkoffResponse> GetTinkoffResponseAsync(IEnumerable<Ticker> tickers, CancellationToken token)
    {
        var request = SerializeToRequest(tickers);

        var content = new HttpRequestMessage
        {
            Content = new StringContent(request, Encoding.UTF8, "application/json"),
            RequestUri = new Uri(_tinkoffUrl),
            Method = HttpMethod.Post
        };
        var response = await _client.SendAsync(content, token);

        response.EnsureSuccessStatusCode();

        var serializedResponse = await response.Content.ReadFromJsonAsync<TinkoffResponse>(cancellationToken: token)
                                 ?? throw new InvalidOperationException("Ошибка получения ответа от tinkoff.com");

        return serializedResponse;
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