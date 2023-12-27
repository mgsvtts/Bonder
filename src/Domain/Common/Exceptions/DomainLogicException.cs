namespace Domain.Common.Exceptions;

public class DomainLogicException : Exception
{
    public DomainLogicException(string message) : base(message)
    {
    }
}