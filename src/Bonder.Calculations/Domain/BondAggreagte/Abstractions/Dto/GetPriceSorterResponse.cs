namespace Domain.BondAggreagte.Abstractions.Dto;
public readonly record struct GetPriceSorterResponse(PageInfo PageInfo, List<Bond> Bonds);