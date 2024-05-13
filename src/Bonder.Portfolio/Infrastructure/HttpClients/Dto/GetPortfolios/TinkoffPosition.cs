using Infrastructure.HttpClients.Dto.Common;
using System.Text.Json.Serialization;

namespace Infrastructure.HttpClients.Dto.GetPortfolios;

public sealed class TinkoffPosition
{
    [JsonPropertyName("instrumentType")]
    public string Type { get; set; }

    [JsonPropertyName("quantity")]
    public TinkoffQuantity Quantity { get; set; }

    [JsonPropertyName("instrumentUid")]
    public string InstrumentId { get; set; }
}