using Domain.BondAggreagte;
using Domain.BondAggreagte.Dto;
using Domain.BondAggreagte.ValueObjects;
using MediatR;

namespace Application.Calculation.Common.CalculationService.Dto;

public record struct CalculationResults
{
    private List<CalculationResult> _results;

    public readonly IReadOnlyList<CalculationResult> Results => _results.AsReadOnly();
    public IEnumerable<Bond> Bonds { get; }
    public IEnumerable<CalculationMoneyResult> PriceSortedBonds { get; }
    public IEnumerable<CalculationMoneyResult> FullIncomeSortedBonds { get; }
    public IEnumerable<CalculationMoneyResult> CouponIncomeSortedBonds { get; }
    public IEnumerable<CalculationMoneyResult> NominalIncomeSortedBonds { get; }

    public readonly void Add(CalculationResult result)
    {
        _results.Add(result);
    }

    public CalculationResults(CalculationRequest request)
    {
        _results = new List<CalculationResult>();

        var incomes = CalculateIncomes(request);

        Bonds = request.Bonds;

        PriceSortedBonds = incomes.OrderBy(x => x.Key.Money.Price)
                                  .Select(x => new CalculationMoneyResult(x.Key, x.Key.Money.Price));

        FullIncomeSortedBonds = incomes.OrderByDescending(x => x.Value.FullIncome)
                                       .Select(x => new CalculationMoneyResult(x.Key, x.Value.FullIncome));

        CouponIncomeSortedBonds = incomes.OrderByDescending(x => x.Value.CouponIncome)
                                         .Select(x => new CalculationMoneyResult(x.Key, x.Value.CouponIncome));
        
        NominalIncomeSortedBonds = incomes.OrderByDescending(x => x.Value.NominalIncome)
                                          .Select(x => new CalculationMoneyResult(x.Key, x.Value.NominalIncome));
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
            dict.Add(bond, bond.GetIncome(request.Options));
        }

        return dict;
    }
}
