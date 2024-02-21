using Application.Calculation.CalculateAll.Services.Dto;
using Domain.BondAggreagte.Abstractions.Dto;
using MediatR;

namespace Application.Calculation.CalculateTickers;
public sealed record CalculateBondsCommand(GetPriceSortedRequest Options, IdType IdType, IEnumerable<string> Ids) : IRequest<CalculateAllResponse>;

public enum IdType
{
    Ticker,
    Uid
}