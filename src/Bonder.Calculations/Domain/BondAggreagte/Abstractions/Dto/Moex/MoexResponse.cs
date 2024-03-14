using Domain.BondAggreagte.ValueObjects.Incomes;

namespace Domain.BondAggreagte.Abstractions.Dto.Moex;

public readonly record struct MoexResponse(List<Coupon> Coupons, List<Amortization> Amortizations);