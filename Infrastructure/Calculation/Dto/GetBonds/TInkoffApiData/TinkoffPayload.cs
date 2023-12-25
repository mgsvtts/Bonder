using System.Text.Json.Serialization;

namespace Infrastructure.Calculation.Dto.GetBonds.TInkoffApiData;

public sealed class TinkoffPayload
{
    [JsonPropertyName("values")]
    public IEnumerable<TinkoffValue> Values { get; set; }
}