using Domain.BondAggreagte;
using Domain.BondAggreagte.Abstractions;
using Domain.BondAggreagte.Abstractions.Dto;
using Domain.BondAggreagte.Dto;
using Domain.BondAggreagte.ValueObjects;
using Domain.BondAggreagte.ValueObjects.Identities;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Infrastructure.Calculation.CalculateAll;
public sealed partial class CachedBondRepository : IBondRepository
{
    private static readonly JsonSerializerOptions _jsonOptions;
    private static readonly DistributedCacheEntryOptions _cacheOptions;

    private readonly BondRepository _decorated;
    private readonly IDistributedCache _cache;

    static CachedBondRepository()
    {
        _cacheOptions = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(10)
        };
        _jsonOptions = new JsonSerializerOptions();
        _jsonOptions.Converters.Add(new BondConverter());
    }

    public CachedBondRepository(BondRepository decorated, IDistributedCache cache)
    {
        _decorated = decorated;
        _cache = cache;
    }

    public async Task<GetPriceSortedResponse> GetPriceSortedAsync(GetPriceSortedRequest filter, IEnumerable<Ticker>? tickers = null, IEnumerable<Guid>? uids = null, bool takeAll = false, CancellationToken token = default)
    {
        if (!IsDefaultRequest(filter, tickers, uids, takeAll))
        {
            return await _decorated.GetPriceSortedAsync(filter, tickers, uids, takeAll, token);
        }

        var key = $"sort-page-{filter.PageInfo.CurrentPage}-{filter.IntervalType}";

        var cachedResponse = await _cache.GetStringAsync(key, token);

        if (!string.IsNullOrEmpty(cachedResponse))
        {
            return JsonSerializer.Deserialize<GetPriceSortedResponse>(cachedResponse, _jsonOptions);
        }

        var response = await _decorated.GetPriceSortedAsync(filter, tickers, uids, takeAll, token);

        await _cache.SetStringAsync(key, JsonSerializer.Serialize(response), _cacheOptions, token);

        return response;
    }

    private static bool IsDefaultRequest(GetPriceSortedRequest filter, IEnumerable<Ticker>? tickers, IEnumerable<Guid>? uids, bool takeAll)
    {
        return (filter.IntervalType == DateIntervalType.TillOfferDate || filter.IntervalType == DateIntervalType.TillMaturityDate) &&
               filter.PriceFrom == 0 &&
               filter.PriceTo == decimal.MaxValue &&
               filter.NominalFrom == 0 &&
               filter.NominalTo == decimal.MaxValue &&
               filter.YearCouponFrom == 0 &&
               filter.YearCouponTo == decimal.MaxValue &&
               filter.RatingFrom == 0 &&
               filter.RatingTo == 10 &&
               filter.IncludeUnknownRatings == true &&
               tickers is null &&
               uids is null &&
               takeAll == false;
    }

    public Task AddAsync(IEnumerable<Bond> bonds, CancellationToken token = default)
    {
        return _decorated.AddAsync(bonds, token);
    }

    public Task<int> CountAsync(CancellationToken token = default)
    {
        return _decorated.CountAsync(token);
    }

    public Task<List<Bond>> GetAllFloatingAsync(CancellationToken token = default)
    {
        return _decorated.GetAllFloatingAsync(token);
    }

    public Task<List<Bond>> GetByTickersAsync(IEnumerable<Ticker> tickers, CancellationToken token = default)
    {
        return _decorated.GetByTickersAsync(tickers, token);
    }

    public Task RefreshAsync(IEnumerable<Ticker> oldBondTickers, CancellationToken token = default)
    {
        return _decorated.RefreshAsync(oldBondTickers, token);
    }

    public Task<List<Bond>> TakeRangeAsync(Range range, CancellationToken token = default)
    {
        return _decorated.TakeRangeAsync(range, token);
    }

    public Task UpdateAsync(Bond bond, CancellationToken token = default)
    {
        return _decorated.UpdateAsync(bond, token);
    }

    public Task<List<Ticker>> UpdateIncomesAsync(IEnumerable<KeyValuePair<Ticker, StaticIncome>> bonds, CancellationToken token = default)
    {
        return _decorated.UpdateIncomesAsync(bonds, token);
    }
}
