using Application.Calculation.CalculateAll.Services;
using Application.Calculation.Common.CalculationService.Dto;
using MediatR;
using System.Runtime.CompilerServices;

namespace Application.Calculation.CalculateAll.Stream;

public sealed class CalculateAllStreamRequestHandler : IStreamRequestHandler<CalculateAllStreamRequest, CalculationResults>
{
    private readonly ICalculateAllService _service;

    public CalculateAllStreamRequestHandler(ICalculateAllService service)
    {
        _service = service;
    }

    public async IAsyncEnumerable<CalculationResults> Handle(CalculateAllStreamRequest request, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            yield return await _service.CalculateAllAsync(request.IncomeRequest, cancellationToken);
        }
    }
}