using System.Text.Json.Serialization;

namespace Infrastructure.Dto.GetPortfolios;

public sealed class TinkoffQuantity
{
    [JsonPropertyName("units")]
    public int Units { get; set; }

    [JsonPropertyName("nanos")]
    public int Nanos { get; set; }
}