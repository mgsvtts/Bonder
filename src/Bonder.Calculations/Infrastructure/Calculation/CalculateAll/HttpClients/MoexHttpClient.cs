using Application.Commands.Calculation.Common.Abstractions;
using Application.Commands.Calculation.Common.Abstractions.Dto;
using Domain.BondAggreagte.ValueObjects.Identities;
using Infrastructure.Calculation.Dto.GetAmortization;
using Mapster;
using Microsoft.AspNetCore.Http.Extensions;
using System.Net.Http.Json;

namespace Infrastructure.Calculation.CalculateAll.HttpClients;

public sealed class MoexHttpClient : IMoexHttpClient
{
    private static readonly Dictionary<string, string> _query = GetQueryParams();

    private readonly HttpClient _client;
    private readonly string _serverUrl;

    public MoexHttpClient(HttpClient client,
                          string serverUrl)
    {
        _client = client;
        _serverUrl = serverUrl;
    }

    public async Task<MoexResponse> GetMoexResponseAsync(Ticker ticker, CancellationToken token = default)
    {
        var content = new HttpRequestMessage
        {
            RequestUri = new Uri(BuildQuery(ticker)),
            Method = HttpMethod.Get
        };

        var response = await _client.SendAsync(content, token);

        response.EnsureSuccessStatusCode();

        var serializedResponse = await response.Content.ReadFromJsonAsync<IEnumerable<MoexItem>>(cancellationToken: token);

        var moexItem = serializedResponse?.FirstOrDefault(x => x.Coupons != null)
                       ?? throw new InvalidOperationException("Ошибка получения ответа от moex.com");

        return moexItem.Adapt<MoexResponse>();
    }

    private string BuildQuery(Ticker ticker)
    {
        return _serverUrl + $"/{ticker}.json" + new QueryBuilder(_query);
    }

    private static Dictionary<string, string> GetQueryParams()
    {
        return new Dictionary<string, string>
        {
            ["from"] = DateOnly.MinValue.ToString(),
            ["till"] = DateOnly.MaxValue.ToString(),
            ["start"] = "0",
            ["limit"] = int.MaxValue.ToString(),
            ["iss.only"] = "amortizations,coupons",
            ["iss.json"] = "extended",
            ["iss.meta"] = "off",
        };
    }
}