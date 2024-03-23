namespace Domain.UserAggregate.ValueObjects;

public readonly record struct UserId(Guid Value)
{
    public override string ToString()
    {
        return Value.ToString();
    }
}