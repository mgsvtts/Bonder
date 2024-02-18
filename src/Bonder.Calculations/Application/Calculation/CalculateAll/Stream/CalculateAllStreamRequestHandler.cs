using Application.Calculation.CalculateAll.Services;
using Application.Calculation.CalculateAll.Services.Dto;
using MediatR;
using System.Runtime.CompilerServices;

namespace Application.Calculation.CalculateAll.Stream;

public sealed class CalculateAllStreamRequestHandler : IStreamRequestHandler<CalculateAllStreamRequest, CalculateAllResponse>
{
    private readonly ICalculateAllService _service;

    public CalculateAllStreamRequestHandler(ICalculateAllService service)
    {
        _service = service;
    }

    public async IAsyncEnumerable<CalculateAllResponse> Handle(CalculateAllStreamRequest request, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            yield return await _service.CalculateAllAsync(request.Request, cancellationToken);
        }
    }
}