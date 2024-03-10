namespace Domain.UserAggregate.ValueObjects.Portfolios;

public readonly record struct AccountId
{
    public string Value { get; }
    public AccountId(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            throw new ArgumentException("AccountId cannot be null or empty", nameof(value));
        }

        Value = value;
    }

    public override string ToString()
    {
        return Value;
    }
}