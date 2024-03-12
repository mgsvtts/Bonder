using System.Text.Json.Serialization;

namespace Infrastructure.Dto.GetOperations;

public sealed class TradeInfo
{
    [JsonPropertyName("trades")]
    public IEnumerable<Trade> Trades { get; set; }
}