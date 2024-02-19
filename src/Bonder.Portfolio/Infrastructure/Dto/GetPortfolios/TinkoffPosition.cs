using System.Text.Json.Serialization;

namespace Infrastructure.Dto.GetPortfolios;

public sealed class TinkoffPosition
{
    [JsonPropertyName("instrumentType")]
    public string Type { get; set; }

    [JsonPropertyName("Quantity")]
    public int Quantity { get; set; }

    [JsonPropertyName("instrumentUid")]
    public string InstrumentId { get; set; }
}
