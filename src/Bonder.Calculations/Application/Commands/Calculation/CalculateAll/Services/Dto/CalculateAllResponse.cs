using Application.Commands.Calculation.Common.CalculationService.Dto;
using Domain.BondAggreagte.Abstractions.Dto;

namespace Application.Commands.Calculation.CalculateAll.Services.Dto;
public readonly record struct CalculateAllResponse(CalculationResults Aggregation, PageInfo PageInfo);