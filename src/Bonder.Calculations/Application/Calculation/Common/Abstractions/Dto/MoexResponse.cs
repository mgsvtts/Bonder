using Domain.BondAggreagte.ValueObjects.Incomes;

namespace Application.Calculation.Common.Abstractions.Dto;

public readonly record struct MoexResponse(List<Coupon> Coupons, List<Amortization> Amortizations);