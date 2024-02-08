using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Calculation.Common.CalculationService.Dto;
using Domain.BondAggreagte.Dto;
using MediatR;

namespace Application.Calculation.CalculateAll.Query;

public sealed record CalculateAllCommand(GetIncomeRequest IncomeRequest) : IRequest<CalculationResults>;