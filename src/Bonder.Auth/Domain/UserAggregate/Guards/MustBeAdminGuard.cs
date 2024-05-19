using Ardalis.GuardClauses;
using Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace Domain.UserAggregate.Guards;
public static class MustBeAdminGuard
{
    public static void NotAdmin(this IGuardClause clause, User user)
    {
        if(user.IsAdmin)
        {
            throw new AuthorizationException("You must be an admin to do this");
        }
    }
}
