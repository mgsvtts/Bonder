namespace Domain.UserAggregate.ValueObjects;

public readonly record struct Tokens(string RefreshToken, string AccessToken)
{
    public static Tokens Empty => new("", "");
}
