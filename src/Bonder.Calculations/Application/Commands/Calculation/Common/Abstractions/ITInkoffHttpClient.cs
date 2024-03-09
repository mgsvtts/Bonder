using Application.Commands.Calculation.Common.Abstractions.Dto;
using Domain.BondAggreagte.ValueObjects;
using Domain.BondAggreagte.ValueObjects.Identities;

namespace Application.Commands.Calculation.Common.Abstractions;

public interface ITInkoffHttpClient
{
    public Task<Dictionary<Ticker, StaticIncome>> GetBondPriceAsync(IEnumerable<Ticker> tickers, CancellationToken token = default);

    public Task<GetBondResponse> GetBondByTickerAsync(Ticker ticker, CancellationToken token = default);
}