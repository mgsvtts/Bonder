using Application.Calculation.Common.CalculationService.Dto;
using Application.Calculation.Common.CalculationService.Extensions;
using Application.Calculation.Common.Interfaces;

namespace Application.Calculation.Common.CalculationService;

public class CalculationService : ICalculationService
{
    public CalculationResults Calculate(CalculationRequest request)
    {
        var results = new CalculationResults(request);

        foreach (var bond in request.Bonds)
        {
            var priceIndex = results.PriceSortedBonds.FindBondIndex(bond.Identity);
            var fullIncomeIndex = results.FullIncomeSortedBonds.FindBondIndex(bond.Identity);
            var ratingIndex = results.RatingSortedBonds.FindBondIndex(bond.Identity);

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