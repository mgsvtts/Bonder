using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.BondAggreagte.Exceptions;
public sealed class BondNotFoundException : Exception
{
    public BondNotFoundException(string id) : base($"Bond with id: {id} not found")
    {
        
    }
}
