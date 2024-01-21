namespace Domain.BondAggreagte.ValueObjects;

public readonly record struct IncomePercents
{
    public decimal NominalPercent { get; }
    public decimal PricePercent { get; }

    private IncomePercents(decimal pricePercent, decimal nominalPercent)
    {
        NominalPercent = nominalPercent;
        PricePercent = pricePercent;
    }

    public static IncomePercents FromAbsoluteValues(decimal price, decimal nominal)
    {
        var nominalIncome = (nominal - price) / nominal;
        var priceIncome = price / nominal;

        return new IncomePercents(priceIncome, nominalIncome);
    }

    public static IncomePercents FromPercents(decimal pricePercent, decimal nominalPercent)
    {
        return new IncomePercents(pricePercent, nominalPercent);
    }
}
