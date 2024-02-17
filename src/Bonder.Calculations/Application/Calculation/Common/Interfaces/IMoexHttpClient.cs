using Domain.BondAggreagte.ValueObjects;

namespace Application.Calculation.Common.Interfaces;

public interface IMoexHttpClient
{
    public Task<List<Coupon>> GetAmortizedCouponsAsync(Ticker ticker, CancellationToken token = default);
}