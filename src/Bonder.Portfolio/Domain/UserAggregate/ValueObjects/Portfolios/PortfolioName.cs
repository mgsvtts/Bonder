using Ardalis.GuardClauses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.UserAggregate.ValueObjects.Portfolios;
public readonly record struct PortfolioName
{
    public string Value { get; }

    public PortfolioName(string value)
    {
        Guard.Against.NullOrEmpty(value, message: "PortfolioName cannot be null or empty");

        Value = value.Trim();
    }
}