using Application.Commands.Calculation.Common.Abstractions;
using Application.Commands.Calculation.Common.CalculationService.Dto;
using Application.Commands.Calculation.Common.CalculationService.Extensions;

namespace Application.Commands.Calculation.Common.CalculationService;

public sealed class CalculationService : ICalculationService
{
    private const int _mixRating = 10;

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

    private static int ConvertIndex(int? rating)
    {
        if (rating is null || rating == -1)
        {
            return _mixRating;
        }

        return _mixRating - rating.Value;
    }
}