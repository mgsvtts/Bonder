using Domain.BondAggreagte.Abstractions.Dto.CalculateAll;

namespace Application.Commands.Calculation.CalculateTickers;

public readonly record struct CalculateBondsByIdsResponse(CalculateAllResponse CalculateResponse, IEnumerable<string> NotFoundBonds);