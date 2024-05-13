using System.Text.Json.Serialization;

namespace Infrastructure.HttpClients.Dto.GetAccounts;

public sealed class TinkoffAccount
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("type")]
    public string Type { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("status")]
    public string Status { get; set; }
}