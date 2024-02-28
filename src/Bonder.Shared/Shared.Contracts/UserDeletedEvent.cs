using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Contracts;
public sealed class UserDeletedEvent
{
    public Guid UserId { get; set; }
}
