using Application.Calculation.Common.CalculationService.Dto;
using Application.Calculation.Common.Interfaces;
using Domain.BondAggreagte.Repositories;
using Infrastructure.Calculation.CalculateAll;
using MediatR;

namespace Application.Calculation.CalculateTickers;

public sealed class CalculateTickersCommandHandler : IRequestHandler<CalculateTickersCommand, CalculationResults>
{
    private readonly ITInkoffHttpClient _httpClient;
    private readonly ICalculationService _calculator;
    private readonly ICacheService _cache;

    public CalculateTickersCommandHandler(ICalculationService calculator, ITInkoffHttpClient httpClient, ICacheService cache)
    {
        _calculator = calculator;
        _httpClient = httpClient;
        _cache = cache;
    }

    public async Task<CalculationResults> Handle(CalculateTickersCommand request, CancellationToken cancellationToken)
    {
        var bonds = await _httpClient.GetBondsByTickersAsync(request.Tickers.DistinctBy(x => x.Value), cancellationToken);

        await _cache.CacheAsync(bonds, cancellationToken);

        return _calculator.Calculate(new CalculationRequest(request.Options, bonds));
    }
}