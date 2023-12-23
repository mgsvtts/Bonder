using Application.Calculation.Common.CalculationService;
using Application.Calculation.Common.Interfaces;
using Domain.BondAggreagte;
using MediatR;

namespace Application.Calculation.CalculateUids;

public sealed class CalculateUidsCommandHandler : IRequestHandler<CalculateUidsCommand, CalculationResult>
{
    private readonly ITinkoffGrpcClient _grpcClient;
    private readonly ICalculator _calculator;

    public CalculateUidsCommandHandler(ITinkoffGrpcClient grpcClient, ICalculator calculator)
    {
        _grpcClient = grpcClient;
        _calculator = calculator;
    }

    public async Task<CalculationResult> Handle(CalculateUidsCommand request, CancellationToken cancellationToken)
    {
        var bonds = new List<Bond>();

        if (request.Uids.Count() == 1)
        {
            bonds.Add(await _grpcClient.GetBondByUidAsync(request.Uids.First(), cancellationToken));
        }
        else
        {
            bonds.AddRange(await _grpcClient.GetBondsByUidsAsync(request.Uids.Distinct(), cancellationToken));
        }

        return _calculator.Calculate(bonds);
    }
}