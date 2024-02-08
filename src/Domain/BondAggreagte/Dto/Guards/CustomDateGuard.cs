using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Ardalis.GuardClauses;

namespace Domain.BondAggreagte.Dto.Guards;

public static class CustomDateGuard
{
    public static (DateOnly DateFrom, DateOnly? DateTo) CustomDateNotSetted(this IGuardClause clause, DateIntervalType type, DateOnly? dateFrom, DateOnly? dateTo)
    {
        dateFrom ??= DateOnly.FromDateTime(DateTime.Now);
        if (type == DateIntervalType.TillCustomDate && dateTo == null)
        {
            throw new ArgumentException($"{nameof(dateTo)} cannot be null", nameof(dateTo));
        }
        if (type == DateIntervalType.TillCustomDate && dateTo > dateFrom)
        {
            throw new ArgumentException($"{nameof(dateTo)} must be less than {nameof(dateFrom)}", nameof(dateTo));
        }

        return (dateFrom.Value, dateTo);
    }
}