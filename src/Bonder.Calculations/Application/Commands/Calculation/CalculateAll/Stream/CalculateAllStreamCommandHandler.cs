using Domain.BondAggreagte.Abstractions;
using Domain.BondAggreagte.Abstractions.Dto.CalculateAll;
using Mediator;
using System.Runtime.CompilerServices;

namespace Application.Commands.Calculation.CalculateAll.Stream;

public sealed class CalculateAllStreamCommandHandler : IStreamCommandHandler<CalculateAllStreamCommand, CalculateAllResponse>
{
    private readonly ICalculateAllService _service;

    public CalculateAllStreamCommandHandler(ICalculateAllService service)
    {
        _service = service;
    }

    public async IAsyncEnumerable<CalculateAllResponse> Handle(CalculateAllStreamCommand request, [EnumeratorCancellation] CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            yield return await _service.CalculateAllAsync(request.Request, token);
        }
    }
}