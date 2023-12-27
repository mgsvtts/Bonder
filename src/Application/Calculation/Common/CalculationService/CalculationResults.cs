using Domain.BondAggreagte;

namespace Application.Calculation.Common.CalculationService;

public record struct CalculationResults
{
    private List<CalculationResult> _results;

    public readonly IReadOnlyList<CalculationResult> Results => _results.AsReadOnly();

    public readonly void Add(CalculationResult result)
    {
        _results.Add(result);
    }

    public CalculationResults()
    {
        _results = new List<CalculationResult>();
    }

    public CalculationResults OrderByPriority()
    {
        _results = _results.OrderBy(x => x.Priority)
                           .ToList();

        return this;
    }
}

public readonly record struct CalculationResult(Bond Bond,
                                                int Priority,
                                                decimal Price,
                                                decimal CouponIncome,
                                                decimal Income);