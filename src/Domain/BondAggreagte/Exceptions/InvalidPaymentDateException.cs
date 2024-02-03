using Domain.Common.Exceptions;

namespace Domain.BondAggreagte.Exceptions;

public sealed class InvalidPaymentDateException : DomainLogicException
{
    public InvalidPaymentDateException(DateOnly paymentDate) : base($"{nameof(paymentDate)} cannot be in the past")
    {
    }
}
