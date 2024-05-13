using Infrastructure.HttpClients.Dto.Common;
using System.Text.Json.Serialization;

namespace Infrastructure.HttpClients.Dto.GetOperations;

public sealed class Trade
{
    [JsonPropertyName("date")]
    public DateTime Date { get; set; }

    [JsonPropertyName("quantity")]
    public int Quantity { get; set; }

    [JsonPropertyName("price")]
    public TinkoffQuantity Price { get; set; }
}