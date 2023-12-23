using Application.Calculation.Common.CalculationService;
using MediatR;

namespace Application.Calculation.CalculateUids;
public sealed record CalculateUidsCommand(IEnumerable<Guid> Uids) : IRequest<CalculationResult>;