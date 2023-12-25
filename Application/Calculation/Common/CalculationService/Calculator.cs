using Application.Calculation.Common.CalculationService.Dto;
using Application.Calculation.Common.Interfaces;

namespace Application.Calculation.Common.CalculationService;

public class Calculator : ICalculator
{
    public CalculationResults Calculate(CalculationRequest request)
    {
        var results = new CalculationResults();

        var couponIncomeList = request.Bonds.OrderByDescending(x => x.GetCouponOnlyIncome(request.Options))
                                            .ToList();

        var priceList = request.Bonds.OrderBy(x => x.Money.Price)
                                     .ToList();

        var incomeList = request.Bonds.OrderByDescending(x => x.GetFullIncome(request.Options))
                                      .ToList();

        foreach (var bond in request.Bonds)
        {
            var priceIndex = priceList.FindIndex(x => x.Id == bond.Id);
            var payoutIndex = couponIncomeList.FindIndex(x => x.Id == bond.Id);
            var mainIncome = incomeList.FindIndex(x => x.Id == bond.Id);

            results.Add(new CalculationResult(bond,
                                              CalculatePriority(priceIndex, payoutIndex, mainIncome),
                                              bond.Money.Price,
                                              bond.GetCouponOnlyIncome(request.Options),
                                              bond.GetFullIncome(request.Options)));
        }

        return results.OrderByPriority();
    }

    private static int CalculatePriority(int priceIndex,
                                         int payoutIndex,
                                         int mainIncomeIndex)
    {
        return priceIndex + payoutIndex + mainIncomeIndex;
    }
}