using Domain.BondAggreagte.ValueObjects.Identities;
using Domain.Common.Exceptions;

namespace Domain.BondAggreagte.Exceptions;

public sealed class IsinLengthException : DomainLogicException
{
    public IsinLengthException(string isin) : base($"Isin lenght must be equal to {Isin.Length}, you gave {isin.Length}")
    {
    }
}