using System.Text.Json.Serialization;

namespace Infrastructure.Calculation.Dto.GetBondPrices;

public sealed class BondLastPrice
{
    [JsonPropertyName("price")]
    public BondPrice Price { get; set; }

    [JsonPropertyName("time")]
    public DateTime Time { get; set; }
}