using Domain.BondAggreagte;

namespace Application.Calculation.Common.Interfaces;

public interface IAllBondsReceiver
{
    int MaxRange { get; }

    Task<IEnumerable<Bond>> ReceiveAsync(Range takeRange, CancellationToken token);
}
