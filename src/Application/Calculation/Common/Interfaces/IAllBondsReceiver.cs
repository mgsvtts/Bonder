using Domain.BondAggreagte;

namespace Application.Calculation.Common.Interfaces;
public interface IAllBondsReceiver
{
    Task<IEnumerable<Bond>> ReceiveAsync(CancellationToken token);
}