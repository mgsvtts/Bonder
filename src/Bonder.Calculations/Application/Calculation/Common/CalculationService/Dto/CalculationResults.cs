using Domain.BondAggreagte;
using Domain.BondAggreagte.ValueObjects.Incomes;

namespace Application.Calculation.Common.CalculationService.Dto;

public record struct CalculationResults
{
    private List<CalculationResult> _results;

    public readonly IReadOnlyList<CalculationResult> Results => _results.AsReadOnly();
    public IDictionary<Bond, FullIncome> Bonds { get; }
    public List<CalculationMoneyResult> PriceSortedBonds { get; }
    public List<CalculationMoneyResult> FullIncomeSortedBonds { get; }

    public CalculationResults(IDictionary<Bond, FullIncome> bonds,
                              IEnumerable<Bond> priceSortedBonds)
    {
        _results = [];

        Bonds = bonds;
        PriceSortedBonds = priceSortedBonds
        .Select(x => new CalculationMoneyResult(x, x.Income.StaticIncome.PricePercent))
        .ToList();

        FullIncomeSortedBonds = bonds.Select(x => new CalculationMoneyResult(x.Key, x.Value.FullIncomePercent))
        .ToList();
    }

    public CalculationResults(CalculationRequest request)
    {
        _results = [];

        Bonds = CalculateIncomes(request);

        PriceSortedBonds = Bonds
        .OrderBy(x => x.Key.Income.StaticIncome.PricePercent)
        .Select(x => new CalculationMoneyResult(x.Key, x.Key.Income.StaticIncome.PricePercent))
        .ToList();

        FullIncomeSortedBonds = Bonds
        .OrderByDescending(x => x.Value.FullIncomePercent)
        .Select(x => new CalculationMoneyResult(x.Key, x.Value.FullIncomePercent))
        .ToList();
    }

    public readonly void Add(CalculationResult result)
    {
        _results.Add(result);
    }

    public CalculationResults OrderByPriority()
    {
        _results = _results
        .OrderBy(x => x.Priority)
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