using Ardalis.GuardClauses;

namespace Domain.BondAggreagte.ValueObjects;

public readonly record struct BondName
{
    public string Value { get; }

    public BondName(string value)
    {
        Guard.Against.NullOrEmpty(value);

        Value = value.Trim();
    }

    public override string ToString()
    {
        return Value;
    }
}
