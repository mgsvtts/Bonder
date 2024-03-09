using Application.Commands.Calculation.Common.Abstractions.Dto;
using Domain.BondAggreagte.ValueObjects.Identities;

namespace Application.Commands.Calculation.Common.Abstractions;

public interface IMoexHttpClient
{
    public Task<MoexResponse> GetMoexResponseAsync(Ticker ticker, CancellationToken token = default);
}