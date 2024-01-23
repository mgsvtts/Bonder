using Domain.BondAggreagte;
using Domain.BondAggreagte.ValueObjects;

namespace Application.Calculation.Common.Interfaces;

public interface IAllBondsReceiver
{
    int MaxRange { get; }

    Task<IEnumerable<KeyValuePair<Ticker, StaticIncome>>> ReceiveAsync(Range takeRange, CancellationToken token);
}
