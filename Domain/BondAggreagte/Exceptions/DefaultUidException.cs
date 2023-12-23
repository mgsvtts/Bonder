namespace Domain.BondAggreagte.Exceptions;

public sealed class DefaultUidException : Exception
{
    public DefaultUidException() : base("You must initialize uid with non-default value")
    {
    }
}