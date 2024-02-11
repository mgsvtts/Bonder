using Application.Calculation.Common.CalculationService.Dto;
using Domain.BondAggreagte.Dto;
using Domain.BondAggreagte.ValueObjects;
using MediatR;

namespace Application.Calculation.CalculateTickers;
public sealed record CalculateTickersCommand(GetIncomeRequest Options, IEnumerable<Ticker> Tickers) : IRequest<CalculationResults>;
