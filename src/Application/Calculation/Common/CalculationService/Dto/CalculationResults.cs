using Domain.BondAggreagte;
using Domain.BondAggreagte.ValueObjects;

namespace Application.Calculation.Common.CalculationService.Dto;

public record struct CalculationResults
{
    private List<CalculationResult> _results;

    public readonly IReadOnlyList<CalculationResult> Results => _results.AsReadOnly();
    public IDictionary<Bond, Income> Bonds { get; }
    public IEnumerable<CalculationMoneyResult> PriceSortedBonds { get; }
    public IEnumerable<CalculationMoneyResult> FullIncomeSortedBonds { get; }
    public IEnumerable<CalculationRatingResult> RatingSortedBonds { get; }

    public readonly void Add(CalculationResult result)
    {
        _results.Add(result);
    }

    public CalculationResults(CalculationRequest request)
    {
        _results = new List<CalculationResult>();

        Bonds = CalculateIncomes(request);

        PriceSortedBonds = Bonds.OrderBy(x => x.Key.Money.Price)
                                .Select(x => new CalculationMoneyResult(x.Key, x.Key.Money.Price));

        FullIncomeSortedBonds = Bonds.OrderByDescending(x => x.Value.FullIncome)
                                     .Select(x => new CalculationMoneyResult(x.Key, x.Value.FullIncome));

        RatingSortedBonds = request.Bonds.OrderByDescending(x => x.Rating)
                                         .Select(x => new CalculationRatingResult(x, x.Rating));
    }

    public CalculationResults OrderByPriority()
    {
        _results = _results.OrderBy(x => x.Priority)
                           .ToList();

        return this;
    }

    private static Dictionary<Bond, Income> CalculateIncomes(CalculationRequest request)
    {
        var dict = new Dictionary<Bond, Income>();

        foreach (var bond in request.Bonds)
        {
            try
            {
                dict.Add(bond, bond.GetIncome(request.Options));
            }
            catch
            {
            }
        }

        return dict;
    }
}