using Application.Calculation.CalculateAll.Services;
using Application.Calculation.CalculateAll.Services.Dto;
using Domain.BondAggreagte.Abstractions.Dto;
using Domain.BondAggreagte.ValueObjects.Identities;
using Infrastructure.Calculation.CalculateAll.Repositories;
using Infrastructure.Common.JsonConverters;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

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
        _jsonOptions.Converters.Add(new CalculateAllResponseConverter());
        _jsonOptions.Converters.Add(new FullIncomeConverter());
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
