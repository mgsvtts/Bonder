using Domain.BondAggreagte.ValueObjects;
using Domain.BondAggreagte.ValueObjects.Identities;

namespace Infrastructure.Calculation.Dto.BondRepository;

public sealed class UpdatePricesParams
{
    public List<string> Tickers { get; } = [];
    public List<decimal> PricePercents { get; } = [];
    public List<decimal> AbsoluteNominals { get; } = [];
    public List<decimal> AbsolutePrices { get; } = [];

    public void Add(KeyValuePair<Ticker, StaticIncome> pair)
    {
        Tickers.Add(pair.Key.ToString());
        PricePercents.Add(pair.Value.PricePercent);
        AbsoluteNominals.Add(pair.Value.AbsoluteNominal);
        AbsolutePrices.Add(pair.Value.AbsolutePrice);
    }
}