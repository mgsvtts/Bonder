using Application.Calculation.Common.CalculationService;
using Application.Calculation.Common.Interfaces;
using Domain.BondAggreagte;
using MediatR;

namespace Application.Calculation.CalculateFigis;

public sealed class CalculateFigisCommandHandler : IRequestHandler<CalculateFigisCommand, CalculationResult>
{
    private readonly ITinkoffGrpcClient _grpcClient;
    private readonly ICalculator _calculator;

    public CalculateFigisCommandHandler(ITinkoffGrpcClient grpcClient, ICalculator calculator)
    {
        _grpcClient = grpcClient;
        _calculator = calculator;
    }

    public async Task<CalculationResult> Handle(CalculateFigisCommand request, CancellationToken cancellationToken)
    {
        var bonds = new List<Bond>();

        if (request.Figis.Count() == 1)
        {
            bonds.Add(await _grpcClient.GetBondByFigiAsync(request.Figis.First(), cancellationToken));
        }
        else
        {
            bonds.AddRange(await _grpcClient.GetBondsByFigisAsync(request.Figis.Distinct(), cancellationToken));
        }

        return _calculator.Calculate(bonds);
    }
}