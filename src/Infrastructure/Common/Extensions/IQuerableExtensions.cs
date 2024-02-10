using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Infrastructure.Common.Extensions;

public static class IQuerableExtensions
{
    public static IQueryable<T> WhereIf<T>(this IQueryable<T> query, bool condition, Expression<Func<T, bool>> result)
    {
        if (condition)
        {
            return query.Where(result);
        }

        return query;
    }
}