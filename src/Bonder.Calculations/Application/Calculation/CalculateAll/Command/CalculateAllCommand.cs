using Application.Calculation.CalculateAll.Services.Dto;
using Domain.BondAggreagte.Abstractions.Dto;
using Mediator;

namespace Application.Calculation.CalculateAll.Command;

public sealed record CalculateAllCommand(GetPriceSortedRequest Request) : ICommand<CalculateAllResponse>;