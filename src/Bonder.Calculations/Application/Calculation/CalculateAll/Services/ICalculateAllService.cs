using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Calculation.Common.CalculationService.Dto;
using Domain.BondAggreagte.Dto;

namespace Application.Calculation.CalculateAll.Services;

public interface ICalculateAllService
{
    public Task<CalculationResults> CalculateAllAsync(GetIncomeRequest request, CancellationToken token = default);
}