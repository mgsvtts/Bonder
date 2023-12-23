using Application.Calculation.Common.Interfaces;
using Domain.BondAggreagte;

namespace Application.Calculation.Common.CalculationService;

public class Calculator : ICalculator
{
    public CalculationResult Calculate(IEnumerable<Bond> bonds)
    {
        var result = new List<CalculatedBond>();

        var couponIncomeList = bonds.OrderByDescending(x => x.GetCouponOnlyIncome())
                                    .ToList();

        var priceList = bonds.OrderBy(x => x.Money.Price)
                             .ToList();

        var incomeList = bonds.OrderByDescending(x => x.GetFullIncome())
                              .ToList();

        foreach (var bond in bonds)
        {
            var priceIndex = priceList.FindIndex(x => x.Id == bond.Id);
            var payoutIndex = couponIncomeList.FindIndex(x => x.Id == bond.Id);
            var mainIncome = incomeList.FindIndex(x => x.Id == bond.Id);

            result.Add(new CalculatedBond(bond, CalculatePriority(priceIndex, payoutIndex, mainIncome)));
        }

        return new CalculationResult(result.OrderBy(x => x.Priority),
                                     priceList,
                                     couponIncomeList,
                                     incomeList);
    }

    private static int CalculatePriority(int priceIndex,
                                         int payoutIndex,
                                         int mainIncomeIndex)
    {
        return priceIndex + payoutIndex + mainIncomeIndex;
    }
}