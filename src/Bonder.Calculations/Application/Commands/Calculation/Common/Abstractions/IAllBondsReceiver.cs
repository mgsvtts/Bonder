using Domain.BondAggreagte.ValueObjects;
using Domain.BondAggreagte.ValueObjects.Identities;

namespace Application.Commands.Calculation.Common.Abstractions;

public interface IAllBondsReceiver
{
    Task<IEnumerable<KeyValuePair<Ticker, StaticIncome>>> ReceiveAsync(CancellationToken token);
}