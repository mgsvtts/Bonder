using Application.Commands.Calculation.CalculateAll.Services.Dto;
using Domain.BondAggreagte.Abstractions.Dto;

namespace Application.Commands.Calculation.CalculateAll.Services;

public interface ICalculateAllService
{
    public Task<CalculateAllResponse> CalculateAllAsync(GetPriceSortedRequest request, CancellationToken token = default);
}