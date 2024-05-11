namespace Domain.UserAggregate.ValueObjects.Users;

public readonly record struct TinkoffToken
{
    public string Value { get; }

    public TinkoffToken(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            throw new ArgumentException("Tinkoff token cannot be null or empty", nameof(value));
        }

        Value = value.Trim();
    }

    public override string ToString()
    {
        return Value;
    }
}