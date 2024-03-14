using Domain.BondAggreagte.Abstractions;
using Domain.BondAggreagte.Abstractions.Dto.CalculateAll;
using Domain.BondAggreagte.Abstractions.Dto.GetPriceSorted;
using Infrastructure.Common.JsonConverters;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace Infrastructure.Calculation.CalculateAll.Cache;

public class CachedCalculateAllService : ICalculateAllService
{
    private static readonly JsonSerializerOptions _jsonOptions;
    private static readonly DistributedCacheEntryOptions _cacheOptions;

    private readonly CalculateAllService _decorated;
    private readonly IDistributedCache _cache;

    static CachedCalculateAllService()
    {
        _cacheOptions = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(10)
        };
        _jsonOptions = new JsonSerializerOptions();
        _jsonOptions.Converters.Add(new BondConverter());
        _jsonOptions.Converters.Add(new FullIncomeConverter());
        _jsonOptions.Converters.Add(new CalculateAllResponseConverter());
    }

    public CachedCalculateAllService(CalculateAllService decorated, IDistributedCache cache)
    {
        _decorated = decorated;
        _cache = cache;
    }

    public async Task<CalculateAllResponse> CalculateAllAsync(GetPriceSortedRequest request, CancellationToken token = default)
    {
        if (!request.IsDefault())
        {
            return await _decorated.CalculateAllAsync(request, token);
        }

        var key = $"service-sort-page-{request.PageInfo.CurrentPage}-{request.IntervalType}";

        var cachedResponse = await _cache.GetStringAsync(key, token);

        if (!string.IsNullOrEmpty(cachedResponse))
        {
            return JsonSerializer.Deserialize<CalculateAllResponse>(cachedResponse, _jsonOptions);
        }

        var response = await _decorated.CalculateAllAsync(request, token);

        await _cache.SetStringAsync(key, JsonSerializer.Serialize(response), _cacheOptions, token);

        return response;
    }
}