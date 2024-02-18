using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.UserAggregate.Exceptions;
public sealed class InvalidTokenException : Exception
{
    public InvalidTokenException() : base("Provided token is invalid")
    { }
}