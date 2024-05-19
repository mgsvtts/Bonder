using Ardalis.GuardClauses;
using Shared.Domain.Common.Guards;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Domain.UserAggregate.ValueObjects;
public readonly record struct UserName
{
    public string Value { get; }

    public UserName(string value)
    {
        Guard.Against.NullOrEmpty(value, message: "UserName cannot be null or empty");
        Guard.Against.Contains(value, ' ', "UserName");

        Value = value.Trim();
    }

    public override string ToString()
    {
        return Value;
    }
}