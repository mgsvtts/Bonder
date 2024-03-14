using Domain.BondAggreagte.ValueObjects.Incomes;

namespace Domain.BondAggreagte.Abstractions;

public interface ITinkoffGrpcClient
{
    public Task<List<Coupon>> GetCouponsAsync(Guid instrumentId, CancellationToken token = default);
}