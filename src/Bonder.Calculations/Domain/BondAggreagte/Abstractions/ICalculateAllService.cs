using Domain.BondAggreagte.Abstractions.Dto.CalculateAll;
using Domain.BondAggreagte.Abstractions.Dto.GetPriceSorted;

namespace Domain.BondAggreagte.Abstractions;

public interface ICalculateAllService
{
    public Task<CalculateAllResponse> CalculateAllAsync(GetPriceSortedRequest request, CancellationToken token);
}