using Ardalis.GuardClauses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Domain.Common.Guards;
public static class ContainsGuard
{
    public static void Contains(this IGuardClause guardClause, string input, string shoudNotContain, [CallerArgumentExpression("input")] string? parameterName = null)
    {
        if (input.Contains(shoudNotContain))
        {
            throw new ArgumentException($"{parameterName ?? "String"} must not contain '{shoudNotContain}'", parameterName ?? nameof(input));
        }
    }

    public static void Contains(this IGuardClause guardClause, string input, char shoudNotContain, [CallerArgumentExpression("input")] string? parameterName = null)
    {
        if (input.Contains(shoudNotContain))
        {
            throw new ArgumentException($"{parameterName ?? "String"} must not contain '{shoudNotContain}'", parameterName ?? nameof(input));
        }
    }
}
