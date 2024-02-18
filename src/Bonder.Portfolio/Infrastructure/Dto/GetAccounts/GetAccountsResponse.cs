using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Infrastructure.Dto.GetAccounts;
public sealed class GetAccountsResponse
{
    [JsonPropertyName("accounts")]
    public IEnumerable<TinkoffAccount> Accounts { get; set; }
}
