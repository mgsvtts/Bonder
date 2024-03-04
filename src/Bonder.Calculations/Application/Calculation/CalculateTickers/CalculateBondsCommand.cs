using Application.Calculation.CalculateAll.Services.Dto;
using Domain.BondAggreagte.Abstractions.Dto;
using Mediator;

namespace Application.Calculation.CalculateTickers;
public sealed record CalculateBondsCommand(GetPriceSortedRequest Options, IdType IdType, IEnumerable<string> Ids) : ICommand<CalculateAllResponse>;

public enum IdType
{
    Ticker,
    Uid
}