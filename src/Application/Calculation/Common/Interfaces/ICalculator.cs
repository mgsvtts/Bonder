using Application.Calculation.Common.CalculationService.Dto;

namespace Application.Calculation.Common.Interfaces;

public interface ICalculator
{
    public CalculationResults Calculate(CalculationRequest request);
}