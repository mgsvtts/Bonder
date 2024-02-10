using Domain.BondAggreagte.Dto;
using Domain.BondAggreagte.ValueObjects;

namespace Domain.BondAggreagte.Abstractions;

public interface IBondRepository
{
    Task<int> CountAsync(CancellationToken token = default);
    Task UpdateRating(BondId id, int? rating, CancellationToken token = default);
    Task<List<Bond>> TakeRangeAsync(Range range, CancellationToken token = default);
    Task UpdateCoupons(IEnumerable<Coupon> coupons, BondId id, CancellationToken token = default);
    Task<List<Bond>> GetAllFloatingAsync(CancellationToken token = default);
    Task RefreshAsync(IEnumerable<Ticker> oldBondTickers, CancellationToken token = default);
    Task<List<Ticker>> UpdateIncomesAsync(IEnumerable<KeyValuePair<Ticker, StaticIncome>> bonds, CancellationToken token = default);
    Task<List<Bond>> GetPriceSortedAsync(GetIncomeRequest filter, IEnumerable<Ticker>? tickers = null, CancellationToken token = default);
    Task<List<Bond>> GetPriceSortedAsync(CancellationToken token = default);
    Task AddAsync(IEnumerable<Bond> bonds, CancellationToken token = default);
}
