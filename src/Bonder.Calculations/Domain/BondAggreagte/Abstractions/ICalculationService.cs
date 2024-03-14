using Domain.BondAggreagte.Abstractions.Dto.CalculateAll;

namespace Domain.BondAggreagte.Abstractions;

public interface ICalculationService
{
    public CalculationResults Calculate(CalculationRequest request);

    CalculationResults Calculate(SortedCalculationRequest request);
}