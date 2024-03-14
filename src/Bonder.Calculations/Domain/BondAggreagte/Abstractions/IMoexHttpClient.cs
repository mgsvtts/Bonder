using Domain.BondAggreagte.Abstractions.Dto.Moex;
using Domain.BondAggreagte.ValueObjects.Identities;

namespace Domain.BondAggreagte.Abstractions;

public interface IMoexHttpClient
{
    public Task<MoexResponse> GetMoexResponseAsync(Ticker ticker, CancellationToken token = default);
}