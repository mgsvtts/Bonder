using Application.Calculation.Common.CalculationService;
using Domain.BondAggreagte;

namespace Application.Calculation.Common.Interfaces;

public interface ICalculator
{
    public CalculationResult Calculate(IEnumerable<Bond> bonds);
}