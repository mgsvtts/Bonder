namespace Domain.BondAggreagte.Exceptions;

public sealed class BondNotFoundException : Exception
{
    public BondNotFoundException(string id) : base($"Bond with id: {id} not found")
    {
    }
}