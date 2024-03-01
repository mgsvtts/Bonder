using Domain.BondAggreagte.ValueObjects;
using Domain.BondAggreagte.ValueObjects.Identities;

namespace Application.Calculation.Common.Abstractions;

public interface IAllBondsReceiver
{
    Task<List<KeyValuePair<Ticker, StaticIncome>>> ReceiveAsync(CancellationToken token);
}