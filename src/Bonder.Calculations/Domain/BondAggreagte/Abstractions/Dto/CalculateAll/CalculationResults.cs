namespace Domain.BondAggreagte.Abstractions.Dto.CalculateAll;

public record struct CalculationResults
{
    private List<CalculationResult> _results = [];

    public readonly IReadOnlyList<CalculationResult> PrioritySortedBonds => _results.AsReadOnly();
    public List<BondWithIncome> BondsWithIncome { get; }
    public List<CalculationMoneyResult> PriceSortedBonds { get; }
    public List<CalculationMoneyResult> FullIncomeSortedBonds { get; }

    //Deserialization constructor
    private CalculationResults(List<CalculationResult> prioritySortedBonds,
                               List<BondWithIncome> bondsWithIncome,
                               List<CalculationMoneyResult> priceSortedBonds,
                               List<CalculationMoneyResult> fullIncomeSortedBonds)
    {
        _results = prioritySortedBonds;
        BondsWithIncome = bondsWithIncome;
        PriceSortedBonds = priceSortedBonds;
        FullIncomeSortedBonds = fullIncomeSortedBonds;
    }

    public CalculationResults(IEnumerable<BondWithIncome> bonds,
                              IEnumerable<Bond> priceSortedBonds)
    {
        BondsWithIncome = bonds.ToList();
        PriceSortedBonds = priceSortedBonds
        .Select(x => new CalculationMoneyResult(x, x.Income.StaticIncome.PricePercent))
        .ToList();

        FullIncomeSortedBonds = bonds.Select(x => new CalculationMoneyResult(x.Bond, x.FullIncome.FullIncomePercent))
        .ToList();
    }

    public CalculationResults(CalculationRequest request)
    {
        BondsWithIncome = CalculateIncomes(request);

        PriceSortedBonds = BondsWithIncome
        .OrderBy(x => x.Bond.Income.StaticIncome.PricePercent)
        .Select(x => new CalculationMoneyResult(x.Bond, x.Bond.Income.StaticIncome.PricePercent))
        .ToList();

        FullIncomeSortedBonds = BondsWithIncome
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