using Application.Commands.Calculation.CalculateAll.Services.Dto;
using Domain.BondAggreagte.Abstractions.Dto;
using Mediator;

namespace Application.Commands.Calculation.CalculateAll.Command;

public sealed record CalculateAllCommand(GetPriceSortedRequest Request) : ICommand<CalculateAllResponse>;