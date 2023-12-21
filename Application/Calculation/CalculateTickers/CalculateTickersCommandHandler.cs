using Application.Calculation.CalculateTickers.Interfaces;
using Application.Calculation.Common.CalculationService;
using MediatR;

namespace Application.Calculation.CalculateTickers;

public sealed class CalculateTickersCommandHandler : IRequestHandler<CalculateTickersCommand, CalculationResult>
{
    private readonly ITinkoffGrpcClient _grpcClient;

    public CalculateTickersCommandHandler(ITinkoffGrpcClient grpcClient)
    {
        _grpcClient = grpcClient;
    }

    public async Task<CalculationResult> Handle(CalculateTickersCommand request, CancellationToken cancellationToken)
    {
        var bonds = await _grpcClient.GetBondsByTickersAsync(request.Tickers, cancellationToken);

        return await CalculationService.CalculateAsync(bonds);
    }
}