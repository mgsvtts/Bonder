using Application.Calculation.Common.CalculationService.Dto;
using Domain.BondAggreagte.ValueObjects;

namespace Application.Calculation.Common.CalculationService.Extensions;

public static class BondsExtensions
{
    public static int FindBondIndex(this IEnumerable<CalculationItem> items, BondId id)
    {
        var count = 0;
        foreach (var item in items)
        {
            if (item.Bond.Identity == id)
            {
                break;
            }

            count++;
        }

        return count;
    }
}