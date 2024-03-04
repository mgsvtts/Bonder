using Application.Calculation.CalculateAll.Services;
using Application.Calculation.CalculateAll.Services.Dto;
using Mediator;
using System.Runtime.CompilerServices;

namespace Application.Calculation.CalculateAll.Stream;

public sealed class CalculateAllStreamCommandHandler : IStreamCommandHandler<CalculateAllStreamCommand, CalculateAllResponse>
{
    private readonly ICalculateAllService _service;

    public CalculateAllStreamCommandHandler(ICalculateAllService service)
    {
        _service = service;
    }

    public async IAsyncEnumerable<CalculateAllResponse> Handle(CalculateAllStreamCommand request, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            yield return await _service.CalculateAllAsync(request.Request, cancellationToken);
        }
    }
}