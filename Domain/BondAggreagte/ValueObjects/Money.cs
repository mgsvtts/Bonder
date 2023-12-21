namespace Domain.BondAggreagte.ValueObjects;

public class Money
{
    public decimal NominalIncome { get; private set; }

    public decimal Price { get; private set; }

    public Money(decimal price, decimal nominal)
    {
        NominalIncome = nominal - price;
        Price = price;
    }
}