using Application.Calculation.Common.CalculationService.Dto;
using Application.Calculation.Common.Interfaces;
using Domain.BondAggreagte;
using Domain.BondAggreagte.Dto;
using Domain.BondAggreagte.Repositories;
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
            var priceSorted = await _bondRepository.GetPriceSortedBondsAsync(cancellationToken);

            var fullIncomeTask = Task.Run(() =>
            {
                return priceSorted.OrderByDescending(x => x.GetIncome(request.IncomeRequest).FullIncome).ToList();
            });

            var ratingSort = await _bondRepository.GetRatingSortedBondsAsync(cancellationToken);

            yield return _calculator.Calculate(new SortedCalculationRequest(request.IncomeRequest, priceSorted, await fullIncomeTask, ratingSort));
        }
    }
}