using Infrastructure.Dto.Common;
using System.Text.Json.Serialization;

namespace Infrastructure.Dto.GetPortfolios;

public sealed class GetTinkoffPortfolioResponse
{
    [JsonPropertyName("totalAmountBonds")]
    public TinkoffQuantity TotalBondPrice { get; set; }

    [JsonPropertyName("positions")]
    public IEnumerable<TinkoffPosition> Positions { get; set; }
}