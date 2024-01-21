using Application.Calculation.Common.CalculationService.Dto;
using Application.Calculation.Common.Interfaces;
using MediatR;

namespace Application.Calculation.CalculateTickers;

public sealed class CalculateTickersCommandHandler : IRequestHandler<CalculateTickersCommand, CalculationResults>
{
    private readonly ITInkoffHttpClient _httpClient;
    private readonly ICalculationService _calculator;

    public CalculateTickersCommandHandler(ICalculationService calculator, ITInkoffHttpClient httpClient)
    {
        _calculator = calculator;
        _httpClient = httpClient;
    }

    public async Task<CalculationResults> Handle(CalculateTickersCommand request, CancellationToken cancellationToken)
    {
        var bonds = await _httpClient.GetBondsByTickersAsync(request.Tickers.DistinctBy(x => x.Value), cancellationToken);

        return _calculator.Calculate(new CalculationRequest(request.Options, bonds));
    }
}
