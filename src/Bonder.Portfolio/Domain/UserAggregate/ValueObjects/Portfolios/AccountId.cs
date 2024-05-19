using Ardalis.GuardClauses;

namespace Domain.UserAggregate.ValueObjects.Portfolios;

public readonly record struct AccountId
{
    public string Value { get; }
    public AccountId(string value)
    {
        Guard.Against.NullOrEmpty(value, message: "AccountId cannot be null or empty");

        Value = value;
    }

    public override string ToString()
    {
        return Value;
    }
}