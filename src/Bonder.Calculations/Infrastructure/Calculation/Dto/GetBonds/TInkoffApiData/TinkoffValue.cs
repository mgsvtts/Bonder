using System.Text.Json.Serialization;

namespace Infrastructure.Calculation.Dto.GetBonds.TInkoffApiData;

public sealed class TinkoffValue
{
    [JsonPropertyName("symbol")]
    public TinkoffSymbol Symbol { get; set; }

    [JsonPropertyName("price")]
    public TinkoffPrice? Price { get; set; }

    [JsonPropertyName("faceValue")]
    public decimal Nominal { get; set; }

    [JsonPropertyName("buyBackDate")]
    public DateTime? OfferDate { get; set; }

    [JsonPropertyName("matDate")]
    public DateTime? MaturityDate { get; set; }

    [JsonPropertyName("callDate")]
    public DateTime? CallDate { get; set; }

    [JsonPropertyName("serial")]
    public bool IsAmortized { get; set; }
}