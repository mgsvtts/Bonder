using System.Text.Json.Serialization;

namespace Infrastructure.Calculation.Dto.GetBonds.TInkoffApiData;

public sealed class TinkoffSymbol
{
    [JsonPropertyName("ticker")]
    public string Ticker { get; set; }

    [JsonPropertyName("securityUids")]
    public TinkoffSecurityUids SecurityUids { get; set; }

    [JsonPropertyName("showName")]
    public string Name { get; set; }
}