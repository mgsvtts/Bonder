using Application.Calculation.Common.CalculationService;
using Domain.BondAggreagte.ValueObjects;
using MediatR;

namespace Application.Calculation.CalculateFigis;
public sealed record CalculateFigisCommand(IEnumerable<Figi> Figis) : IRequest<CalculationResult>;