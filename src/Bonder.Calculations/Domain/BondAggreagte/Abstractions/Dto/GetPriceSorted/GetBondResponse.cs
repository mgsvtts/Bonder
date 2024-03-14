using Domain.BondAggreagte.ValueObjects;
using Domain.BondAggreagte.ValueObjects.Identities;

namespace Domain.BondAggreagte.Abstractions.Dto.GetPriceSorted;

public readonly record struct GetBondResponse(BondId BondId,
                                              string Name,
                                              StaticIncome Income,
                                              Dates Dates,
                                              bool IsAmortized);