using Shared.Domain.Common;

namespace Domain.BondAggreagte.Abstractions.Dto.GetPriceSorted;
public readonly record struct GetPriceSortedResponse(PageInfo? PageInfo, List<Bond> Bonds);