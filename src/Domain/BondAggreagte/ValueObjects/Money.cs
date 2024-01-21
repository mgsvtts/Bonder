namespace Domain.BondAggreagte.ValueObjects;

public readonly record struct Money
{
    public decimal NominalIncome { get; }

    public decimal Price { get; }

    public Money(decimal price, decimal nominal, bool calculateIncome = true)
    {
        NominalIncome = calculateIncome ? nominal - price : nominal;
        Price = price;
    }
}
