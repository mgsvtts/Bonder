using Domain.BondAggreagte.ValueObjects;

namespace Application.Calculation.Common.Abstractions;

public interface IAllBondsReceiver
{
    int GetMaxRange();

    Task<IEnumerable<KeyValuePair<Ticker, StaticIncome>>> ReceiveAsync(Range takeRange, CancellationToken token);
}