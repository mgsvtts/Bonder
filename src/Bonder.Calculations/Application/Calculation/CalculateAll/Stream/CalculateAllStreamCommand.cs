using Application.Calculation.CalculateAll.Services.Dto;
using Domain.BondAggreagte.Abstractions.Dto;
using Mediator;

namespace Application.Calculation.CalculateAll.Stream;
public sealed record CalculateAllStreamCommand(GetPriceSortedRequest Request) : IStreamCommand<CalculateAllResponse>;