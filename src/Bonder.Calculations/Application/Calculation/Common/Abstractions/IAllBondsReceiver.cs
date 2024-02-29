using Domain.BondAggreagte.ValueObjects;
using Domain.BondAggreagte.ValueObjects.Identities;

namespace Application.Calculation.Common.Abstractions;

public interface IAllBondsReceiver
{
    int GetMaxRange();

    Task<List<KeyValuePair<Ticker, StaticIncome>>> ReceiveAsync(Range takeRange, CancellationToken token);
}