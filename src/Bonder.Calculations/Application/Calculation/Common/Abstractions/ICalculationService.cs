using Application.Calculation.Common.CalculationService.Dto;

namespace Application.Calculation.Common.Abstractions;

public interface ICalculationService
{
    public CalculationResults Calculate(CalculationRequest request);

    CalculationResults Calculate(SortedCalculationRequest request);
}