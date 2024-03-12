using Infrastructure.Dto.Common;
using System.Text.Json.Serialization;

namespace Infrastructure.Dto.GetOperations;

public sealed class Trade
{
    [JsonPropertyName("date")]
    public DateTime Date { get; set; }

    [JsonPropertyName("quantity")]
    public int Quantity { get; set; }

    [JsonPropertyName("price")]
    public TinkoffQuantity Price { get; set; }
}