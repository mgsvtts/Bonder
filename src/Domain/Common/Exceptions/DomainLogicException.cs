using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Common.Exceptions;
public class DomainLogicException : Exception
{
    public DomainLogicException(string message) : base(message)
    {

    }
}
