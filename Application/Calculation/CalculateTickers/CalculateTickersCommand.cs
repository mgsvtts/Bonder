using Application.Calculation.Common.CalculationService;
using Domain.BondAggreagte.ValueObjects;
using MediatR;

namespace Application.Calculation.CalculateTickers;
public sealed record CalculateTickersCommand(IEnumerable<Ticker> Tickers) : IRequest<CalculationResult>;