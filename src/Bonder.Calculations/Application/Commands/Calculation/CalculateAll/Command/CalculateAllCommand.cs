using Domain.BondAggreagte.Abstractions.Dto.CalculateAll;
using Domain.BondAggreagte.Abstractions.Dto.GetPriceSorted;
using Mediator;

namespace Application.Commands.Calculation.CalculateAll.Command;

public sealed record CalculateAllCommand(GetPriceSortedRequest Request) : ICommand<CalculateAllResponse>;