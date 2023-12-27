namespace Domain.BondAggreagte.ValueObjects;

public readonly record struct Money
{
    public decimal NominalIncome { get; }

    public decimal Price { get; }

    public Money(decimal price, decimal nominal)
    {
        NominalIncome = nominal - price;
        Price = price;
    }
}