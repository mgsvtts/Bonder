using Application.Calculation.CalculateAll.Services;
using Application.Calculation.CalculateAll.Services.Dto;
using Mediator;

namespace Application.Calculation.CalculateAll.Command;

public sealed class CalculateAllCommandHandler : ICommandHandler<CalculateAllCommand, CalculateAllResponse>
{
    private readonly ICalculateAllService _service;

    public CalculateAllCommandHandler(ICalculateAllService service)
    {
        _service = service;
    }

    public async ValueTask<CalculateAllResponse> Handle(CalculateAllCommand request, CancellationToken cancellationToken)
    {
        return await _service.CalculateAllAsync(request.Request, cancellationToken);
    }
}