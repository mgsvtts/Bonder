using System.Text.Json.Serialization;

namespace Infrastructure.Dto.GetAccounts;

public sealed class GetAccountsResponse
{
    [JsonPropertyName("accounts")]
    public IEnumerable<TinkoffAccount> Accounts { get; set; }
}