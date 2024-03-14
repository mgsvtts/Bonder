using Domain.BondAggreagte.Abstractions.Dto.CalculateAll;
using Domain.BondAggreagte.Abstractions.Dto.GetPriceSorted;
using Mediator;

namespace Application.Commands.Calculation.CalculateAll.Stream;
public sealed record CalculateAllStreamCommand(GetPriceSortedRequest Request) : IStreamCommand<CalculateAllResponse>;