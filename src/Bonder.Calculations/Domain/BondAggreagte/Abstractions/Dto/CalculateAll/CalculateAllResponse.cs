using Shared.Domain.Common;

namespace Domain.BondAggreagte.Abstractions.Dto.CalculateAll;
public readonly record struct CalculateAllResponse(CalculationResults Aggregation, PageInfo PageInfo);