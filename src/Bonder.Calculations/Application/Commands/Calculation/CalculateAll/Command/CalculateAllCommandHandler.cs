using Domain.BondAggreagte.Abstractions;
using Domain.BondAggreagte.Abstractions.Dto.CalculateAll;
using Mediator;

namespace Application.Commands.Calculation.CalculateAll.Command;

public sealed class CalculateAllCommandHandler : ICommandHandler<CalculateAllCommand, CalculateAllResponse>
{
    private readonly ICalculateAllService _service;

    public CalculateAllCommandHandler(ICalculateAllService service)
    {
        _service = service;
    }

    public async ValueTask<CalculateAllResponse> Handle(CalculateAllCommand request, CancellationToken token)
    {
        return await _service.CalculateAllAsync(request.Request, token);
    }
}