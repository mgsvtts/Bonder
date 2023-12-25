using Infrastructure.Calculation.Dto.GetBonds.TInkoffApiData;
using System.Text.Json.Serialization;

namespace Infrastructure.Calculation.Dto.GetBonds;

public sealed class GetBondsByTickersResponse
{
    [JsonPropertyName("payload")]
    public TinkoffPayload Payload { get; set; }
}