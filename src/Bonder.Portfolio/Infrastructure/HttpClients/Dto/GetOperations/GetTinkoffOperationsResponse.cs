using System.Text.Json.Serialization;

namespace Infrastructure.HttpClients.Dto.GetOperations;

public sealed class GetTinkoffOperationsResponse
{
    [JsonPropertyName("hasNext")]
    public bool HasNext { get; set; }

    [JsonPropertyName("nextCursor")]
    public string? NextCursor { get; set; }

    [JsonPropertyName("items")]
    public IEnumerable<TinkoffOperation> Operations { get; set; }
}