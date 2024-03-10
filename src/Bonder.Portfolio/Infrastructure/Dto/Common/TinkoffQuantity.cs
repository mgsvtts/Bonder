using System.Text.Json.Serialization;

namespace Infrastructure.Dto.Common;

public sealed class TinkoffQuantity
{
    [JsonPropertyName("units")]
    public int Units { get; set; }

    [JsonPropertyName("nano")]
    public int Nanos { get; set; }

    public decimal ToDecimal()
    {
        return Units + Nanos / 1000000000m;
    }
}