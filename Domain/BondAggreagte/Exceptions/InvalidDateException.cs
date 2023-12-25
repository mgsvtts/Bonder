using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.BondAggreagte.Exceptions;
public sealed class InvalidDateException : Exception
{
    public InvalidDateException() : base("Date cannot be in the past")
    {
        
    }
}
