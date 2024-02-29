using Application.Calculation.Common.Abstractions.Dto;
using Domain.BondAggreagte.ValueObjects.Identities;

namespace Application.Calculation.Common.Abstractions;

public interface IMoexHttpClient
{
    public Task<MoexResponse> GetMoexResponseAsync(Ticker ticker, CancellationToken token = default);
}