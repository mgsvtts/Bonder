using Domain.BondAggreagte.ValueObjects;
using Domain.Common.Exceptions;

namespace Domain.BondAggreagte.Exceptions;

public class IsinLengthException : DomainLogicException
{
    public IsinLengthException(string isin) : base($"Isin lenght must be equal to {Isin.Length}, you gave {isin.Length}")
    {
    }
}