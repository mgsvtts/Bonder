namespace Domain.BondAggreagte.ValueObjects;
public readonly record struct StaticIncome
{
    public readonly decimal NominalPercent { get; }
    public readonly decimal PricePercent { get; }
    public readonly decimal AbsolutePrice { get; }
    public readonly decimal AbsoluteNominal { get; }

    private StaticIncome(decimal pricePercent,
                         decimal nominalPercent,
                         decimal price,
                         decimal nominal)
    {
        PricePercent = pricePercent;
        NominalPercent = nominalPercent;
        AbsolutePrice = price;
        AbsoluteNominal = nominal;
    }

    public static StaticIncome FromAbsoluteValues(decimal price, decimal nominal)
    {
        var nominalIncome = (nominal - price) / nominal;
        var priceIncome = price / nominal;

        return new StaticIncome(priceIncome, nominalIncome, price, nominal);
    }

    public static StaticIncome FromPercents(decimal pricePercent, 
                                            decimal nominalPercent,
                                            decimal price,
                                            decimal nominal)
    {
        return new StaticIncome(pricePercent, nominalPercent, price, nominal);
    }

    public static StaticIncome None => new StaticIncome(0, 0, 0, 0);
}
