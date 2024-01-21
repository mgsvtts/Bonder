using Domain.BondAggreagte.ValueObjects;

namespace Application.Calculation.Common.Interfaces;

public interface ITinkoffGrpcClient
{
    public Task<List<Coupon>> GetBondCouponsAsync(Guid instrumentId, CancellationToken token);
}
