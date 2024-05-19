using Ardalis.GuardClauses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Domain.Common.Guards;
public static class EqualGuard
{
    public static void Equal<T>(this IGuardClause clause,
        T first,
        T second,
        string? message = null,
        [CallerArgumentExpression("first")] string? firstParameterName = null,
        [CallerArgumentExpression("second")] string? secondParameterName = null)
    {
        if(first!.Equals(second))
        {
            throw new ArgumentException(message ?? $"{firstParameterName ?? nameof(first)} must not be equal to {secondParameterName ?? nameof(second)}");
        }
    }

    public static void NotEqual<T>(this IGuardClause clause,
        T first,
        T second,
        string? message = null,
        [CallerArgumentExpression("first")] string? firstParameterName = null,
        [CallerArgumentExpression("second")] string? secondParameterName = null)
    {
        if (!first!.Equals(second))
        {
            throw new ArgumentException(message ?? $"{firstParameterName ?? nameof(first)} must not equal to {secondParameterName ?? nameof(second)}");
        }
    }
}
