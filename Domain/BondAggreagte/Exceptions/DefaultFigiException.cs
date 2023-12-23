namespace Domain.BondAggreagte.Exceptions;

public sealed class DefaultFigiException : Exception
{
    public DefaultFigiException() : base("You must initialize figi with non-default value")
    {
    }
}