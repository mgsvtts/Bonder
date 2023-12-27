using Domain.Common.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.BondAggreagte.Exceptions;
public sealed class InvalidPaymentDateException : DomainLogicException
{
    public InvalidPaymentDateException(DateTime paymentDate) : base($"{nameof(paymentDate)} cannot be in the past")
    {
        
    }
}
