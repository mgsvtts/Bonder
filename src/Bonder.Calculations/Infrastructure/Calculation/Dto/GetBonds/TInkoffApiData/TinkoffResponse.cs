using System.Text.Json.Serialization;

namespace Infrastructure.Calculation.Dto.GetBonds.TInkoffApiData;

public sealed class TinkoffResponse
{
    [JsonPropertyName("payload")]
    public TinkoffPayload Payload { get; set; }
}