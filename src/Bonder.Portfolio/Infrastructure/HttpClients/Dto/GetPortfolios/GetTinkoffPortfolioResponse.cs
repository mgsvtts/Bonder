using Infrastructure.HttpClients.Dto.Common;
using System.Text.Json.Serialization;

namespace Infrastructure.HttpClients.Dto.GetPortfolios;

public sealed class GetTinkoffPortfolioResponse
{
    [JsonPropertyName("totalAmountBonds")]
    public TinkoffQuantity TotalBondPrice { get; set; }

    [JsonPropertyName("totalAmountShares")]
    public TinkoffQuantity TotalSharePrice { get; set; }

    [JsonPropertyName("totalAmountEtf")]
    public TinkoffQuantity TotalEtfPrice { get; set; }

    [JsonPropertyName("totalAmountCurrencies")]
    public TinkoffQuantity TotalCurrencyPrice { get; set; }

    [JsonPropertyName("totalAmountFutures")]
    public TinkoffQuantity TotalFuturePrice { get; set; }

    [JsonPropertyName("totalAmountPortfolio")]
    public TinkoffQuantity TotalPortfolioPrice { get; set; }

    [JsonPropertyName("positions")]
    public IEnumerable<TinkoffPosition> Positions { get; set; }
}