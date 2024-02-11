using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Calculation.Common.CalculationService.Dto;
using Application.Calculation.Common.Interfaces;
using Domain.BondAggreagte.Abstractions;
using Domain.BondAggreagte.Dto;

namespace Application.Calculation.CalculateAll.Services;

public class CalculateAllService : ICalculateAllService
{
    private readonly IBondRepository _bondRepository;
    private readonly ICalculationService _calculator;

    public CalculateAllService(IBondRepository bondRepository, ICalculationService calculator)
    {
        _bondRepository = bondRepository;
        _calculator = calculator;
    }

    public async Task<CalculationResults> CalculateAllAsync(GetIncomeRequest request, CancellationToken token = default)
    {
        var priceSorted = await _bondRepository.GetPriceSortedAsync(request, token: token);

        var fullIncomeSorted = priceSorted.OrderByDescending(x => x.GetIncomeOnDate(request).FullIncomePercent)
        .ToList();

        return _calculator.Calculate(new SortedCalculationRequest(request, priceSorted, fullIncomeSorted));
    }
}