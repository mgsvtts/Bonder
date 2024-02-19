using Domain.BondAggreagte.ValueObjects;

namespace Application.Calculation.Common.Abstractions;

public interface IAllBondsReceiver
{
    int GetMaxRange();

    Task<List<KeyValuePair<Ticker, StaticIncome>>> ReceiveAsync(Range takeRange, CancellationToken token);
}