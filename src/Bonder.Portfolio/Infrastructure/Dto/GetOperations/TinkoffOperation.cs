using Infrastructure.Dto.Common;
using System.Text.Json.Serialization;

namespace Infrastructure.Dto.GetOperations;

public sealed class TinkoffOperation
{
    [JsonPropertyName("payment")]
    public TinkoffQuantity Payment { get; set; }

    [JsonPropertyName("state")]
    public string State { get; set; }

    [JsonPropertyName("type")]
    public string Type { get; set; }

    [JsonPropertyName("date")]
    public DateTime Date { get; set; }
}