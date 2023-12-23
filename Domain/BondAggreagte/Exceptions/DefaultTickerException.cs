namespace Domain.BondAggreagte.Exceptions;

public sealed class DefaultTickerException : Exception
{
    public DefaultTickerException() : base("You must initialize ticker with non-default value")
    {
    }
}