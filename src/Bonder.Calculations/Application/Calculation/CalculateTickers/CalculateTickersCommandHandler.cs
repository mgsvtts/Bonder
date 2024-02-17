using Application.Calculation.Common.CalculationService.Dto;
using Application.Calculation.Common.Interfaces;
using Domain.BondAggreagte.Abstractions;
using MediatR;

namespace Application.Calculation.CalculateTickers;

public sealed class CalculateTickersCommandHandler : IRequestHandler<CalculateTickersCommand, CalculationResults>
{
    private readonly IBondRepository _repository;
    private readonly ICalculationService _calculator;

    public CalculateTickersCommandHandler(ICalculationService calculator, IBondRepository repository)
    {
        _calculator = calculator;
        _repository = repository;
    }

    public async Task<CalculationResults> Handle(CalculateTickersCommand request, CancellationToken cancellationToken)
    {
        var bonds = await _repository.GetPriceSortedAsync(request.Options, request.Tickers, cancellationToken);

        return _calculator.Calculate(new CalculationRequest(request.Options, bonds));
    }
}