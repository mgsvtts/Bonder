using Domain.BondAggreagte.Exceptions;

namespace Domain.BondAggreagte.ValueObjects.Identities;
public readonly record struct Isin
{
    public const int Length = 12;

    public string Value { get; }

    public Isin(string value)
    {
        if (value.Length != Length)
        {
            throw new IsinLengthException(value);
        }

        Value = value.Trim().ToUpper();
    }
}