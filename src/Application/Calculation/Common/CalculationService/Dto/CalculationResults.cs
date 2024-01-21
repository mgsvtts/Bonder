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

    public CalculationResults(IDictionary<Bond, Income> bonds,
                              IEnumerable<Bond> priceSortedBonds)
    {
        _results = new List<CalculationResult>();

        Bonds = bonds;
        PriceSortedBonds = priceSortedBonds.Select(x => new CalculationMoneyResult(x, x.Money.Price));
        FullIncomeSortedBonds = bonds.Select(x => new CalculationMoneyResult(x.Key, x.Value.FullIncome));;
    }

    public CalculationResults(CalculationRequest request)
    {
        _results = new List<CalculationResult>();

        Bonds = CalculateIncomes(request);

        PriceSortedBonds = Bonds.OrderBy(x => x.Key.Money.Price)
                                .Select(x => new CalculationMoneyResult(x.Key, x.Key.Money.Price));

        FullIncomeSortedBonds = Bonds.OrderByDescending(x => x.Value.FullIncome)
                                     .Select(x => new CalculationMoneyResult(x.Key, x.Value.FullIncome));;
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

    public static Dictionary<Bond, Income> CalculateIncomes(CalculationRequest request)
    {
        var dict = new Dictionary<Bond, Income>();

        foreach (var bond in request.Bonds)
        {
            dict.Add(bond, bond.GetIncome(request.Options));
        }

        return dict;
    }
}
