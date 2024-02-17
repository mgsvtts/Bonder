using Application.Calculation.Common.CalculationService.Dto;
using Domain.BondAggreagte.Dto;
using MediatR;

namespace Application.Calculation.CalculateAll.Stream;
public sealed record CalculateAllStreamRequest(GetIncomeRequest IncomeRequest) : IStreamRequest<CalculationResults>;