using Domain.BondAggreagte.Abstractions.Dto.CalculateAll;
using Domain.BondAggreagte.Abstractions.Dto.GetPriceSorted;
using Mediator;

namespace Application.Commands.Calculation.CalculateTickers;
public sealed record CalculateBondsByIdsCommand(GetPriceSortedRequest Options, IdType IdType, IEnumerable<string> Ids) : ICommand<CalculateBondsByIdsResponse>;

public enum IdType
{
    Ticker,
    Uid
}