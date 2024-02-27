using Application.Calculation.CalculateAll.Services;
using Application.Calculation.CalculateAll.Services.Dto;
using MediatR;

namespace Application.Calculation.CalculateAll.Command;

public sealed class CalculateAllCommandHandler : IRequestHandler<CalculateAllCommand, CalculateAllResponse>
{
    private readonly ICalculateAllService _service;

    public CalculateAllCommandHandler(ICalculateAllService service)
    {
        _service = service;
    }

    public async Task<CalculateAllResponse> Handle(CalculateAllCommand request, CancellationToken cancellationToken)
    {
        return await _service.CalculateAllAsync(request.Request, cancellationToken);
    }
}