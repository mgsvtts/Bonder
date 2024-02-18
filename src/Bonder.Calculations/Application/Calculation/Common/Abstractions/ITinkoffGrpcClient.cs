using Domain.BondAggreagte.ValueObjects;

namespace Application.Calculation.Common.Abstractions;

public interface ITinkoffGrpcClient
{
    public Task<List<Coupon>> GetCouponsAsync(Guid instrumentId, CancellationToken token = default);
}