using System.Text.Json.Serialization;

namespace Infrastructure.HttpClients.Dto.GetOperations;

public sealed class TradeInfo
{
    [JsonPropertyName("trades")]
    public IEnumerable<Trade> Trades { get; set; }
}