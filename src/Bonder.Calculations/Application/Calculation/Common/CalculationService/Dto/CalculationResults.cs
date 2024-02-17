using Domain.BondAggreagte;
using Domain.BondAggreagte.ValueObjects;

namespace Application.Calculation.Common.CalculationService.Dto;

public record struct CalculationResults
{
    private List<CalculationResult> _results;

    public readonly IReadOnlyList<CalculationResult> Results => _results.AsReadOnly();
    public IDictionary<Bond, FullIncome> Bonds { get; }
    public IEnumerable<CalculationMoneyResult> PriceSortedBonds { get; }
    public IEnumerable<CalculationMoneyResult> FullIncomeSortedBonds { get; }

    public CalculationResults(IDictionary<Bond, FullIncome> bonds,
                              IEnumerable<Bond> priceSortedBonds)
    {
        _results = new List<CalculationResult>();

        Bonds = bonds;
        PriceSortedBonds = priceSortedBonds.Select(x => new CalculationMoneyResult(x, x.Income.StaticIncome.AbsolutePrice));
        FullIncomeSortedBonds = bonds.Select(x => new CalculationMoneyResult(x.Key, x.Value.FullIncomePercent));
    }

    public CalculationResults(CalculationRequest request)
    {
        _results = new List<CalculationResult>();

        Bonds = CalculateIncomes(request);

        PriceSortedBonds = Bonds.OrderBy(x => x.Key.Income.StaticIncome.AbsolutePrice)
                                .Select(x => new CalculationMoneyResult(x.Key, x.Key.Income.StaticIncome.AbsolutePrice));

        FullIncomeSortedBonds = Bonds.OrderByDescending(x => x.Value.FullIncomePercent)
                                     .Select(x => new CalculationMoneyResult(x.Key, x.Value.FullIncomePercent));
    }

    public readonly void Add(CalculationResult result)
    {
        _results.Add(result);
    }

    public CalculationResults OrderByPriority()
    {
        _results = _results.OrderBy(x => x.Priority)
                           .ToList();

        return this;
    }

    public static Dictionary<Bond, FullIncome> CalculateIncomes(CalculationRequest request)
    {
        var dict = new Dictionary<Bond, FullIncome>();

        foreach (var bond in request.Bonds)
        {
            dict.Add(bond, bond.GetIncomeOnDate(request.Options));
        }

        return dict;
    }
}