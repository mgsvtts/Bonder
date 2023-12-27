using System.Text.Json.Serialization;

namespace Infrastructure.Calculation.Dto.GetBonds;

public sealed class GetBondsByTickersRequest
{
    [JsonPropertyName("sortType")]
    public string SortType { get; set; } = "ByName";

    [JsonPropertyName("orderType")]
    public string OrderType { get; set; } = "Asc";

    [JsonPropertyName("country")]
    public string Country { get; set; } = "All";

    [JsonPropertyName("tickers")]
    public IEnumerable<string> Tickers { get; set; }
}