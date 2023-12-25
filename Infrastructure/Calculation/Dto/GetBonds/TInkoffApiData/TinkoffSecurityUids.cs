using System.Text.Json.Serialization;

namespace Infrastructure.Calculation.Dto.GetBonds.TInkoffApiData;

public sealed class TinkoffSecurityUids
{
    [JsonPropertyName("instrumentUid")]
    public Guid InstrumentUid { get; set; }
}