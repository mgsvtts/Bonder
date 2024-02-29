using Domain.BondAggreagte.ValueObjects;
using Domain.BondAggreagte.ValueObjects.Identities;

namespace Application.Calculation.Common.Abstractions.Dto;

public readonly record struct GetBondResponse(BondId BondId,
                                              string Name,
                                              StaticIncome Income,
                                              Dates Dates,
                                              bool IsAmortized);