using Ardalis.GuardClauses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Domain.Common.Guards;

public static class GuardExtensions
{
    public static void Any<T>(this IGuardClause clause,
        IEnumerable<T> items,
        Func<T, bool>? filter = null,
        string? message = null,
        [CallerArgumentExpression("items")] string? parameterName = null)
    {
        var hasItems = false;
        if (filter is null)
        {
            hasItems = items.Any();
        }
        else
        {
            hasItems = items.Any(filter);
        }

        if (hasItems)
        {
            throw new ArgumentException(message ?? "Items must be empty", parameterName ?? nameof(items));
        }
    }
}