using Ardalis.GuardClauses;

namespace Domain.UserAggregate.ValueObjects.Users;

public readonly record struct TinkoffToken
{
    public string Value { get; }

    public TinkoffToken(string value)
    {
        Guard.Against.NullOrEmpty(value, "Tinkoff token cannot be null or empty");

        Value = value.Trim();
    }

    public override string ToString()
    {
        return Value;
    }
}