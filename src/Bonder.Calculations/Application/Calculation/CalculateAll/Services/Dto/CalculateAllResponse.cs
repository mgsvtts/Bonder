using Application.Calculation.Common.CalculationService.Dto;
using Domain.BondAggreagte.Abstractions.Dto;

namespace Application.Calculation.CalculateAll.Services.Dto;
public readonly record struct CalculateAllResponse(CalculationResults Results, PageInfo PageInfo);