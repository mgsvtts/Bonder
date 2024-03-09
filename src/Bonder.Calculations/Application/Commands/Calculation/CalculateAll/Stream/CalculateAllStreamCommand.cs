using Application.Commands.Calculation.CalculateAll.Services.Dto;
using Domain.BondAggreagte.Abstractions.Dto;
using Mediator;

namespace Application.Commands.Calculation.CalculateAll.Stream;
public sealed record CalculateAllStreamCommand(GetPriceSortedRequest Request) : IStreamCommand<CalculateAllResponse>;