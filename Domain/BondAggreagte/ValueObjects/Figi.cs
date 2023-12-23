using Domain.BondAggreagte.Exceptions;

namespace Domain.BondAggreagte.ValueObjects;
public readonly record struct Figi
{
    public readonly string Value { get; }

    public Figi(string value)
    {
        value = value.Trim();
        if (value.Length != 12)
        {
            throw new FigiLengthException(value);
        }

        Value = value;
    }
}