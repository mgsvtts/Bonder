using Application.Calculation.Common.Abstractions;
using Domain.BondAggreagte.ValueObjects;
using Infrastructure.Calculation.Dto.GetAmortization;
using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Http.Extensions;
using System.Net.Http.Json;

namespace Infrastructure.Calculation.CalculateAll;

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

    public async Task<List<Coupon>> GetAmortizedCouponsAsync(Ticker ticker, CancellationToken token = default)
    {
        var content = new HttpRequestMessage
        {
            RequestUri = new Uri(BuildQuery(ticker)),
            Method = HttpMethod.Get
        };

        var response = await _client.SendAsync(content, token);

        response.EnsureSuccessStatusCode();

        var serializedResponse = await response.Content.ReadFromJsonAsync<IEnumerable<MoexItem>>(cancellationToken: token);

        return MapToCoupons(serializedResponse);
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

    private List<Coupon> MapToCoupons(IEnumerable<MoexItem>? moexItems)
    {
        var moexItem = moexItems?.FirstOrDefault(x => x.Coupons != null)
                                 ?? throw new InvalidOperationException("Ошибка получения ответа от moex.com");

        var coupons = moexItem.Coupons.Adapt<List<Coupon>>();

        for (var i = 0; i < coupons.Count; i++)
        {
            foreach (var amortization in moexItem.Amortizations)
            {
                if (coupons[i].PaymentDate == amortization.Date && amortization.Payment != null)
                {
                    coupons[i] = coupons[i] with { Payout = coupons[i].Payout + amortization.Payment.Value };
                }
            }
        }

        return coupons;
    }
}