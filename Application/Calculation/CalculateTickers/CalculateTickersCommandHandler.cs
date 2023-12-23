using Application.Calculation.Common.CalculationService;
using Application.Calculation.Common.Interfaces;
using MediatR;

namespace Application.Calculation.CalculateTickers;

public sealed class CalculateTickersCommandHandler : IRequestHandler<CalculateTickersCommand, CalculationResult>
{
    private readonly ITinkoffGrpcClient _grpcClient;
    private readonly ICalculator _calculator;

    public CalculateTickersCommandHandler(ITinkoffGrpcClient grpcClient, ICalculator calculator)
    {
        _grpcClient = grpcClient;
        _calculator = calculator;
    }

    public async Task<CalculationResult> Handle(CalculateTickersCommand request, CancellationToken cancellationToken)
    {
        var bonds = new List<Domain.BondAggreagte.Bond>();

        if (request.Tickers.Count() == 1)
        {
            bonds.Add(await _grpcClient.GetBondByTickerAsync(request.Tickers.First(), cancellationToken));
        }
        else
        {
            bonds.AddRange(await _grpcClient.GetBondsByTickersAsync(request.Tickers.Distinct(), cancellationToken));
        }

        return _calculator.Calculate(bonds);
    }
}