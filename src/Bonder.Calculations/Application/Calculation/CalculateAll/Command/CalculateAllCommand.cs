using Application.Calculation.Common.CalculationService.Dto;
using Domain.BondAggreagte.Dto;
using MediatR;

namespace Application.Calculation.CalculateAll.Command;

public sealed record CalculateAllCommand(GetIncomeRequest IncomeRequest) : IRequest<CalculationResults>;