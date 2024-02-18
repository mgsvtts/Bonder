namespace Domain.UserAggregate.Exceptions;

public sealed class InvalidTokenException : Exception
{
    public InvalidTokenException() : base("Provided token is invalid")
    { }
}