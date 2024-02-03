using Application.Calculation.Common.CalculationService.Dto;
using Application.Calculation.Common.Interfaces;
using MediatR;

namespace Application.Calculation.CalculateTickers;

public sealed class CalculateTickersCommandHandler : IRequestHandler<CalculateTickersCommand, CalculationResults>
{
    private readonly IBondBuilder _builder;
    private readonly ICalculationService _calculator;

    public CalculateTickersCommandHandler(ICalculationService calculator, IBondBuilder builder)
    {
        _calculator = calculator;
        _builder = builder;
    }

    public async Task<CalculationResults> Handle(CalculateTickersCommand request, CancellationToken cancellationToken)
    {
        var bonds = await _builder.BuildAsync(request.Tickers.DistinctBy(x => x.Value), cancellationToken);

        return _calculator.Calculate(new CalculationRequest(request.Options, bonds));
    }
}
