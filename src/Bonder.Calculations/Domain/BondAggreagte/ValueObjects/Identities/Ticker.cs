namespace Domain.BondAggreagte.ValueObjects.Identities;
public readonly record struct Ticker
{
    public readonly string Value { get; init; }

    public Ticker(string value)
    {
        Value = value.Trim().ToUpper();
    }

    public override string ToString()
    {
        return Value;
    }
}