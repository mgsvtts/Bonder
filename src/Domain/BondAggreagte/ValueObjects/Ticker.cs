namespace Domain.BondAggreagte.ValueObjects;
public readonly record struct Ticker
{
    public readonly string Value { get; }

    public Ticker(string value)
    {
        Value = value.Trim().ToUpper();
    }
}