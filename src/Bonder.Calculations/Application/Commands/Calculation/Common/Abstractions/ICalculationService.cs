using Application.Commands.Calculation.Common.CalculationService.Dto;

namespace Application.Commands.Calculation.Common.Abstractions;

public interface ICalculationService
{
    public CalculationResults Calculate(CalculationRequest request);

    CalculationResults Calculate(SortedCalculationRequest request);
}