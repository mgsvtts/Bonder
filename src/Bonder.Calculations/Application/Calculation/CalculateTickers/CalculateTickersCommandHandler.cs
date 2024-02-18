using Application.Calculation.CalculateAll.Services.Dto;
using Application.Calculation.Common.Abstractions;
using Application.Calculation.Common.CalculationService.Dto;
using Domain.BondAggreagte.Abstractions;
using MediatR;

namespace Application.Calculation.CalculateTickers;

public sealed class CalculateTickersCommandHandler : IRequestHandler<CalculateTickersCommand, CalculateAllResponse>
{
    private readonly IBondRepository _repository;
    private readonly ICalculationService _calculator;

    public CalculateTickersCommandHandler(ICalculationService calculator, IBondRepository repository)
    {
        _calculator = calculator;
        _repository = repository;
    }

    public async Task<CalculateAllResponse> Handle(CalculateTickersCommand request, CancellationToken cancellationToken)
    {
        var bonds = await _repository.GetPriceSortedAsync(request.Options, request.Tickers, cancellationToken);

        return new CalculateAllResponse(_calculator.Calculate(new CalculationRequest(request.Options, bonds.Bonds)), bonds.PageInfo);
    }
}