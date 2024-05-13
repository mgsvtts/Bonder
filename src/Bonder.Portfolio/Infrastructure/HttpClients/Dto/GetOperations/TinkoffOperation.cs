using Infrastructure.HttpClients.Dto.Common;
using System.Text.Json.Serialization;

namespace Infrastructure.HttpClients.Dto.GetOperations;

public sealed class TinkoffOperation
{
    [JsonPropertyName("payment")]
    public TinkoffQuantity Payment { get; set; }

    [JsonPropertyName("price")]
    public TinkoffQuantity ItemPrice { get; set; }

    [JsonPropertyName("commission")]
    public TinkoffQuantity ItemCommission { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("description")]
    public string Description { get; set; }

    [JsonPropertyName("instrumentKind")]
    public string InstrumentKind { get; set; }

    [JsonPropertyName("instrumentUid")]
    public string InstrumentId { get; set; }

    [JsonPropertyName("quantity")]
    public int Quantity { get; set; }

    [JsonPropertyName("quantityRest")]
    public int RestQuantity { get; set; }

    [JsonPropertyName("state")]
    public string State { get; set; }

    [JsonPropertyName("type")]
    public string Type { get; set; }

    [JsonPropertyName("date")]
    public DateTime Date { get; set; }

    [JsonPropertyName("tradesInfo")]
    public TradeInfo? TradeInfo { get; set; }
}