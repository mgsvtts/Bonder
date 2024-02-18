using Application.Calculation.CalculateAll.Services.Dto;
using Domain.BondAggreagte.Abstractions.Dto;

namespace Application.Calculation.CalculateAll.Services;

public interface ICalculateAllService
{
    public Task<CalculateAllResponse> CalculateAllAsync(GetPriceSortedRequest request, CancellationToken token = default);
}