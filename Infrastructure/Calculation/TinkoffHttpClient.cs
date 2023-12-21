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
            Content = new StringContent(JsonSerializer.Serialize(new { InstrumentId = bondId }), Encoding.UTF8, "application/json"),
            RequestUri = new Uri(_tinkoffUrl + "/GetLastPrices"),
            Method = HttpMethod.Post
        };

        var response = await _client.SendAsync(content, token);

        response.EnsureSuccessStatusCode();

        var node = JsonNode.Parse(await response.Content.ReadAsStringAsync(token));
        var bondLastPrices = JsonSerializer.Deserialize<IEnumerable<BondLastPrice>>(node["lastPrices"]);

        var price = bondLastPrices.OrderByDescending(x => x.Time).First().Price;

        var stringPrice = $"{price.Units}{price.Nano.ToString().First()}.{price.Nano.ToString().Remove(0, 1)}";
        return decimal.Parse(stringPrice);
    }
}