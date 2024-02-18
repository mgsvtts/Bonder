using Application.Calculation.CalculateAll.Services.Dto;
using Domain.BondAggreagte.Abstractions.Dto;
using MediatR;

namespace Application.Calculation.CalculateAll.Command;

public sealed record CalculateAllCommand(GetPriceSortedRequest Request) : IRequest<CalculateAllResponse>;