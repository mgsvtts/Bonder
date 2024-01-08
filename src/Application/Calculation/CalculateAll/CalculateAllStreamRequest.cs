using Application.Calculation.Common.CalculationService.Dto;
using MediatR;

namespace Application.Calculation.CalculateAll;
public sealed record CalculateAllStreamRequest() : IStreamRequest<CalculationResults>;