using Application.Calculation.Common.CalculationService.Dto;
using Application.Calculation.Common.Interfaces;
using Domain.BondAggreagte;
using Domain.BondAggreagte.ValueObjects;

namespace Application.Calculation.Common.CalculationService;

public class Incomes
{
    public decimal CouponOnlyIncome { get; set; }
}

public class Calculator : ICalculator
{
    public CalculationResults Calculate(CalculationRequest request)
    {
        var results = new CalculationResults();
        var bondIncomes = CalculateIncomes(request);

        var couponIncomeList = bondIncomes.OrderByDescending(x => x.Value.CouponOnlyIncome)
                                          .Select(x => x.Key)
                                          .ToList();

        var priceList = bondIncomes.OrderBy(x => x.Key.Money.Price)
                                    .Select(x => x.Key)
                                    .ToList();

        var fullIncomeList = bondIncomes.OrderByDescending(x => x.Key.Money.NominalIncome)
                                        .Select(x => x.Key)
                                        .ToList();

        foreach (var bondIncome in bondIncomes)
        {
            var priceIndex = priceList.FindIndex(x => x.Id == bondIncome.Key.Id);
            var payoutIndex = couponIncomeList.FindIndex(x => x.Id == bondIncome.Key.Id);
            var fullIncomeIndex = fullIncomeList.FindIndex(x => x.Id == bondIncome.Key.Id);

            results.Add(new CalculationResult(bondIncome.Key,
                                              CalculatePriority(priceIndex, payoutIndex, fullIncomeIndex),
                                              bondIncome.Key.Money.Price,
                                              bondIncome.Value.CouponOnlyIncome,
                                              bondIncome.Key.Money.NominalIncome));
        }

        return results.OrderByPriority();
    }

    private static Dictionary<Bond, Incomes> CalculateIncomes(CalculationRequest request)
    {
        var dict = new Dictionary<Bond, Incomes>();

        foreach (var bond in request.Bonds)
        {
            dict.Add(bond, new Incomes
            {
                CouponOnlyIncome = bond.GetCouponOnlyIncome(request.Options)
            });
        }

        return dict;
    }

    private static int CalculatePriority(int priceIndex,
                                         int payoutIndex,
                                         int fullIncomeIndex)
    {
        return priceIndex + payoutIndex + fullIncomeIndex;
    }
}