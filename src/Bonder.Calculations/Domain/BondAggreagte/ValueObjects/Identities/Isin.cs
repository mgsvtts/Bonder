using Ardalis.GuardClauses;
using Domain.BondAggreagte.Exceptions;
using Shared.Domain.Common.Guards;

namespace Domain.BondAggreagte.ValueObjects.Identities;
public readonly record struct Isin
{
    public const int Length = 12;

    public string Value { get; init; }

    public Isin(string value)
    {
        Guard.Against.NotEqual(value.Length, Length, $"Isin length must be equal to {Length} but you gave {value.Length}");

        Value = value.Trim().ToUpper();
    }

    public override string ToString()
    {
        return Value;
    }
}