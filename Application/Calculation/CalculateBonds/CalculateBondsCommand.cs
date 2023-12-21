using Application.Calculation.Common.CalculationService;
using Domain.BondAggreagte;
using MediatR;

namespace Application.Calculation.CalculateBonds;
public sealed record CalculateBondsCommand(IEnumerable<Bond> Bonds) : IRequest<CalculationResult>;