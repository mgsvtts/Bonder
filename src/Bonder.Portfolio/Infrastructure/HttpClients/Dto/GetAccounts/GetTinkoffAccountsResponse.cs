using System.Text.Json.Serialization;

namespace Infrastructure.HttpClients.Dto.GetAccounts;

public sealed class GetTinkoffAccountsResponse
{
    [JsonPropertyName("accounts")]
    public IEnumerable<TinkoffAccount> Accounts { get; set; }
}