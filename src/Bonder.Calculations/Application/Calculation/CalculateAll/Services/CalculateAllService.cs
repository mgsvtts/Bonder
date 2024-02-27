using Application.Calculation.CalculateAll.Services.Dto;
using Application.Calculation.Common.Abstractions;
using Application.Calculation.Common.CalculationService.Dto;
using Domain.BondAggreagte.Abstractions;
using Domain.BondAggreagte.Abstractions.Dto;

namespace Application.Calculation.CalculateAll.Services;

public sealed class CalculateAllService : ICalculateAllService
{
    private readonly IBondRepository _bondRepository;
    private readonly ICalculationService _calculator;

    public CalculateAllService(IBondRepository bondRepository, ICalculationService calculator)
    {
        _bondRepository = bondRepository;
        _calculator = calculator;
    }

    public async Task<CalculateAllResponse> CalculateAllAsync(GetPriceSortedRequest request, CancellationToken token = default)
    {
        var paginatedBonds = await _bondRepository.GetPriceSortedAsync(request, token: token);

        var fullIncomeSorted = paginatedBonds.Bonds
        .OrderByDescending(x => x.GetIncomeOnDate(request).FullIncomePercent)
        .ToList();

        return new CalculateAllResponse(_calculator.Calculate(new SortedCalculationRequest(request, paginatedBonds.Bonds, fullIncomeSorted)),
                                        paginatedBonds.PageInfo.Value);
    }
}