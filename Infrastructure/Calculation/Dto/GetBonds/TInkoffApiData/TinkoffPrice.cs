using System.Text.Json.Serialization;

namespace Infrastructure.Calculation.Dto.GetBonds.TInkoffApiData;

public sealed class TinkoffPrice
{
    [JsonPropertyName("value")]
    public decimal Value { get; set; }
}