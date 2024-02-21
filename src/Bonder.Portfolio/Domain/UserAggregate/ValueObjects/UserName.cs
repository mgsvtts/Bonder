namespace Domain.UserAggregate.ValueObjects;
public readonly record struct UserName
{
    public string Name { get; }

    public UserName(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            throw new ArgumentNullException(nameof(name), "User name cannot be empty");
        }

        Name = name;    
    }
}