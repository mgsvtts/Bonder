using System.Linq.Expressions;

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