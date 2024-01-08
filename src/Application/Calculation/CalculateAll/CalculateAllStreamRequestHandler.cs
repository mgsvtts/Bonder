using Application.Calculation.Common.CalculationService.Dto;
using Application.Calculation.Common.Interfaces;
using Domain.BondAggreagte.Dto;
using MediatR;
using System.Runtime.CompilerServices;

namespace Application.Calculation.CalculateAll;

public sealed class CalculateAllStreamRequestHandler : IStreamRequestHandler<CalculateAllStreamRequest, CalculationResults>
{
    private readonly ICalculationService _calculator;

    public CalculateAllStreamRequestHandler(ICalculationService calculator)
    {
        _calculator = calculator;
    }

    public async IAsyncEnumerable<CalculationResults> Handle(CalculateAllStreamRequest request, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            yield return _calculator.Calculate(new CalculationRequest(new GetIncomeRequest(DateIntervalType.TillDate, DateTime.Now.AddYears(2)), AllBonds.State));
        }
    }
}