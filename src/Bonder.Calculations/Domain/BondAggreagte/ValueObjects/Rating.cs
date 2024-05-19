using Ardalis.GuardClauses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.BondAggreagte.ValueObjects;
public readonly record struct Rating
{
    public const int Max = 10;

    public int Value { get; }
    public Rating(int value)
    {
        Guard.Against.NegativeOrZero(value);

        Value = value;
    }

    public override string ToString()
    {
        return Value.ToString();
    }
}
