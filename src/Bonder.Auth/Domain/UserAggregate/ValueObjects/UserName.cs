namespace Domain.UserAggregate.ValueObjects;

public readonly record struct UserName
{
    public string Name { get; }

    public UserName(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            throw new ArgumentException("UserName cannot be null or empty");
        }

        Name = value;
    }
}