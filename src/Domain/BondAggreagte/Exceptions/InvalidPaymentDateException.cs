using Domain.Common.Exceptions;

namespace Domain.BondAggreagte.Exceptions;

public sealed class InvalidPaymentDateException : DomainLogicException
{
    public InvalidPaymentDateException(DateTime paymentDate) : base($"{nameof(paymentDate)} cannot be in the past")
    {
    }
}