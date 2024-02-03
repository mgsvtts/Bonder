using Application.Calculation.Common.CalculationService.Dto;
using Application.Calculation.Common.Interfaces;
using Domain.BondAggreagte.Abstractions;
using MediatR;
using System.Runtime.CompilerServices;

namespace Application.Calculation.CalculateAll;

public sealed class CalculateAllStreamRequestHandler : IStreamRequestHandler<CalculateAllStreamRequest, CalculationResults>
{
    private readonly ICalculationService _calculator;
    private readonly IBondRepository _bondRepository;

    public CalculateAllStreamRequestHandler(ICalculationService calculator, IBondRepository bondRepository)
    {
        _calculator = calculator;
        _bondRepository = bondRepository;
    }

    public async IAsyncEnumerable<CalculationResults> Handle(CalculateAllStreamRequest request, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            var priceSorted = await _bondRepository.GetPriceSortedAsync(cancellationToken);

            var fullIncomeSorted = priceSorted.OrderByDescending(x => x.GetIncomeOnDate(request.IncomeRequest).FullIncomePercent)
                                              .ToList();

            yield return _calculator.Calculate(new SortedCalculationRequest(request.IncomeRequest, priceSorted, fullIncomeSorted));
        }
    }
}
