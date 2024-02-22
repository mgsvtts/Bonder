namespace Domain.BondAggreagte.Abstractions.Dto;
public readonly record struct GetPriceSortedResponse(PageInfo? PageInfo, List<Bond> Bonds);