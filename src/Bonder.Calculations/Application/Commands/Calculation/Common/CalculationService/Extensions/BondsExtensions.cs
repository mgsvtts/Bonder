using Domain.BondAggreagte.Abstractions.Dto.CalculateAll;
using Domain.BondAggreagte.ValueObjects.Identities;

namespace Application.Commands.Calculation.Common.CalculationService.Extensions;

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