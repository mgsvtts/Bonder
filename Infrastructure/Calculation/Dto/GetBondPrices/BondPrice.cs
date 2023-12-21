using System.Text.Json.Serialization;

namespace Infrastructure.Calculation.Dto.GetBondPrices;

public sealed class BondPrice
{
    [JsonPropertyName("units")]
    public string Units { get; set; }

    [JsonPropertyName("nano")]
    public decimal Nano { get; set; }
}