using Domain.BondAggreagte;
using Domain.BondAggreagte.ValueObjects;

namespace Application.Calculation.Common.Interfaces;

public interface IAllBondsReceiver
{
    int GetMaxRange();
    Task<IEnumerable<KeyValuePair<Ticker, StaticIncome>>> ReceiveAsync(Range takeRange, CancellationToken token);
}
