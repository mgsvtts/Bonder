using Domain.BondAggreagte;
using System.Linq;

namespace Application.Calculation.Common.CalculationService.Dto;

public record struct CalculationResults
{
    private List<CalculationResult> _results = [];

    public readonly IReadOnlyList<CalculationResult> Results => _results.AsReadOnly();
    public List<BondWithIncome> Bonds { get; }
    public List<CalculationMoneyResult> PriceSortedBonds { get; }
    public List<CalculationMoneyResult> FullIncomeSortedBonds { get; }

    public CalculationResults(IEnumerable<BondWithIncome> bonds,
                              IEnumerable<Bond> priceSortedBonds)
    {
        Bonds = bonds.ToList();
        PriceSortedBonds = priceSortedBonds
        .Select(x => new CalculationMoneyResult(x, x.Income.StaticIncome.PricePercent))
        .ToList();

        FullIncomeSortedBonds = bonds.Select(x => new CalculationMoneyResult(x.Bond, x.FullIncome.FullIncomePercent))
        .ToList();
    }

    public CalculationResults(CalculationRequest request)
    {
        Bonds = CalculateIncomes(request);

        PriceSortedBonds = Bonds
        .OrderBy(x => x.Bond.Income.StaticIncome.PricePercent)
        .Select(x => new CalculationMoneyResult(x.Bond, x.Bond.Income.StaticIncome.PricePercent))
        .ToList();

        FullIncomeSortedBonds = Bonds
        .OrderByDescending(x => x.FullIncome.FullIncomePercent)
        .Select(x => new CalculationMoneyResult(x.Bond, x.FullIncome.FullIncomePercent))
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

    public static List<BondWithIncome> CalculateIncomes(CalculationRequest request)
    {
        return request.Bonds
        .Select(bond => new BondWithIncome(bond, bond.GetIncomeOnDate(request.Options)))
        .ToList();
    }
}