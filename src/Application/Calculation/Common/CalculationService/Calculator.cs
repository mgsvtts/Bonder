using Application.Calculation.Common.CalculationService.Dto;
using Application.Calculation.Common.CalculationService.Extensions;
using Application.Calculation.Common.Interfaces;

namespace Application.Calculation.Common.CalculationService;

public class Calculator : ICalculator
{
    public CalculationResults Calculate(CalculationRequest request)
    {
        var results = new CalculationResults(request);

        foreach (var bond in request.Bonds)
        {
            var priceIndex = results.PriceSortedBonds.FindBondIndex(bond.Id);
            var fullIncomeIndex = results.FullIncomeSortedBonds.FindBondIndex(bond.Id);
            var ratingIndex = results.RatingSortedBonds.FindBondIndex(bond.Id);

            results.Add(new CalculationResult(bond,
                                              CalculatePriority(priceIndex, fullIncomeIndex, ratingIndex)));
        }

        return results.OrderByPriority();
    }

    private static int CalculatePriority(int priceIndex,
                                         int fullIncomeIndex,
                                         int ratingIndex)
    {
        return priceIndex + fullIncomeIndex + ratingIndex;
    }
}