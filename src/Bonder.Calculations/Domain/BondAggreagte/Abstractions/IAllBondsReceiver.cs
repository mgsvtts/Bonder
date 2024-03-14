using Domain.BondAggreagte.ValueObjects;
using Domain.BondAggreagte.ValueObjects.Identities;

namespace Domain.BondAggreagte.Abstractions;

public interface IAllBondsReceiver
{
    Task<IEnumerable<KeyValuePair<Ticker, StaticIncome>>> ReceiveAsync(CancellationToken token);
}