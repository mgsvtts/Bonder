using Domain.BondAggreagte.ValueObjects;
using Domain.BondAggreagte;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Calculation.Common.CalculationService.Dto;

namespace Application.Calculation.Common.CalculationService.Extensions;
public static class BondsExtensions
{
    public static int FindBondIndex(this IEnumerable<CalculationMoneyResult> items, Ticker id)
    {
        var count = 0;
        foreach (var item in items)
        {
            if (item.Bond.Id == id)
            {
                break;
            }

            count++;
        }

        return count;
    }
}
