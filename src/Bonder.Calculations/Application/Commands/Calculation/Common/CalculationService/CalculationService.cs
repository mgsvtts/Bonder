using Application.Commands.Calculation.Common.CalculationService.Extensions;
using Domain.BondAggreagte.Abstractions;
using Domain.BondAggreagte.Abstractions.Dto.CalculateAll;
using Domain.BondAggreagte.ValueObjects;

namespace Application.Commands.Calculation.Common.CalculationService;

public sealed class CalculationService : ICalculationService
{
    public CalculationResults Calculate(CalculationRequest request)
    {
        var results = new CalculationResults(request);

        foreach (var bond in request.Bonds)
        {
            var ratingIndex = ConvertIndex(bond.Rating);
            var priceIndex = results.PriceSortedBonds.FindBondIndex(bond.Identity);
            var fullIncomeIndex = results.FullIncomeSortedBonds.FindBondIndex(bond.Identity);

            results.Add(new CalculationResult(bond,
                                              CalculatePriority(priceIndex, fullIncomeIndex, ratingIndex)));
        }

        return results.OrderByPriority();
    }

    public CalculationResults Calculate(SortedCalculationRequest request)
    {
        var incomes = CalculationResults.CalculateIncomes(new CalculationRequest(request.Options, request.PriceSortedBonds));
        var results = new CalculationResults(incomes, request.PriceSortedBonds);

        foreach (var bond in request.PriceSortedBonds)
        {
            var ratingIndex = ConvertIndex(bond.Rating);
            var priceIndex = request.PriceSortedBonds.IndexOf(bond);
            var fullIncomeIndex = request.FullIncomeSortedBonds.IndexOf(bond);

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

    private static int ConvertIndex(Rating? rating)
    {
        if (rating is null || rating?.Value == -1)
        {
            return Rating.Max;
        }

        return Rating.Max - rating.Value.Value;
    }
}