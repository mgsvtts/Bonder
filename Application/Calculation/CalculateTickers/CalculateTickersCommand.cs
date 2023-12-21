using Application.Calculation.Common.CalculationService;
using MediatR;

namespace Application.Calculation.CalculateTickers;
public sealed record CalculateTickersCommand(IEnumerable<string> Tickers) : IRequest<CalculationResult>;