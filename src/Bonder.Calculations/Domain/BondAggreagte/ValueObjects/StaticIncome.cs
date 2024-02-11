namespace Domain.BondAggreagte.ValueObjects;
public readonly record struct StaticIncome
{
    public static StaticIncome None => new StaticIncome(0, 0, 0);

    public readonly decimal NominalPercent { get; }
    public readonly decimal AbsolutePrice { get; }
    public readonly decimal AbsoluteNominal { get; }

    private StaticIncome(decimal nominalPercent,
                         decimal price,
                         decimal nominal)
    {
        NominalPercent = nominalPercent;
        AbsolutePrice = price;
        AbsoluteNominal = nominal;
    }

    public static StaticIncome FromAbsoluteValues(decimal price, decimal nominal)
    {
        var nominalIncome = (nominal - price) / nominal;

        return new StaticIncome(nominalIncome, price, nominal);
    }

    public static StaticIncome FromPercents(decimal nominalPercent,
                                            decimal price,
                                            decimal nominal)
    {
        return new StaticIncome(nominalPercent, price, nominal);
    }
}
