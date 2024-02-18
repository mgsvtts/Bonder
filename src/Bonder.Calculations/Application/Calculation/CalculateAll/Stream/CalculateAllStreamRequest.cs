using Application.Calculation.CalculateAll.Services.Dto;
using Domain.BondAggreagte.Abstractions.Dto;
using MediatR;

namespace Application.Calculation.CalculateAll.Stream;
public sealed record CalculateAllStreamRequest(GetPriceSortedRequest Request) : IStreamRequest<CalculateAllResponse>;