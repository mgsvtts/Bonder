using Application.Calculation.Common.CalculationService;
using MediatR;

namespace Application.Calculation.CalculateBonds;

public sealed class CalculateBondsCommandHandler : IRequestHandler<CalculateBondsCommand, CalculationResult>
{
    public async Task<CalculationResult> Handle(CalculateBondsCommand request, CancellationToken cancellationToken)
    {
        return await CalculationService.CalculateAsync(request.Bonds);
    }
}