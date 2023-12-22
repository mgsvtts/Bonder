using Application.Calculation.CalculateTickers.Interfaces;
using Infrastructure.Calculation.Dto.GetBondPrices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Infrastructure.Calculation;

public class TinkoffHttpClient : ITInkoffHttpClient
{
    private readonly HttpClient _client;
    private readonly string _tinkoffUrl;

    public TinkoffHttpClient(HttpClient client, string token, string tinkoffUrl)
    {
        _client = client;
        _tinkoffUrl = tinkoffUrl;

        _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
    }

    public async Task<decimal> GetBondPriceAsync(string bondId, CancellationToken token = default)
    {
        var content = new HttpRequestMessage
        {
            Content = new StringContent(JsonSerializer.Serialize(new { instrumentId = new List<string> { bondId } }), Encoding.UTF8, "application/json"),
            RequestUri = new Uri(_tinkoffUrl + "/GetLastPrices"),
            Method = HttpMethod.Post
        };

        var response = await _client.SendAsync(content, token);

        response.EnsureSuccessStatusCode();

        var bondLastPrices = await ParseLastPricesAsync(response, token);

        var price = bondLastPrices.OrderByDescending(x => x.Time).First().Price;

        return ParsePrice(price);
    }

    private static async Task<List<BondLastPrice>?> ParseLastPricesAsync(HttpResponseMessage response, CancellationToken token)
    {
        var node = JsonNode.Parse(await response.Content.ReadAsStringAsync(token));

        return JsonSerializer.Deserialize<List<BondLastPrice>>(node["lastPrices"]);
    }

    private static decimal ParsePrice(BondPrice price)
    {
        var stringPrice = $"{price.Units}{price.Nano.ToString()[0]}.{price.Nano.ToString().Remove(0, 1)}";
        return decimal.Parse(stringPrice);
    }
}