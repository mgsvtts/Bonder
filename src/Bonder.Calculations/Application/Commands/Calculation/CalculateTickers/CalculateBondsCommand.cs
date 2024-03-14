using Domain.BondAggreagte.Abstractions.Dto.CalculateAll;
using Domain.BondAggreagte.Abstractions.Dto.GetPriceSorted;
using Mediator;

namespace Application.Commands.Calculation.CalculateTickers;
public sealed record CalculateBondsCommand(GetPriceSortedRequest Options, IdType IdType, IEnumerable<string> Ids) : ICommand<CalculateAllResponse>;

public enum IdType
{
    Ticker,
    Uid
}