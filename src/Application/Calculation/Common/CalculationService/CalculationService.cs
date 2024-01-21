using Application.Calculation.Common.CalculationService.Dto;
using Application.Calculation.Common.CalculationService.Extensions;
using Application.Calculation.Common.Interfaces;
using Domain.BondAggreagte;

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

    public CalculationResults Calculate(SortedCalculationRequest request)
    {
        var incomes = CalculationResults.CalculateIncomes(new CalculationRequest(request.Options, request.PriceSortedBonds));
        var results = new CalculationResults
        (
            incomes,
            request.PriceSortedBonds,
            request.RatingSortedBonds
        );

        foreach (var bond in request.PriceSortedBonds)
        {
            var priceIndex = request.PriceSortedBonds.IndexOf(bond);
            var fullIncomeIndex = request.FullIncomeSortedBonds.IndexOf(bond);
            var ratingIndex = request.RatingSortedBonds.IndexOf(bond);

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