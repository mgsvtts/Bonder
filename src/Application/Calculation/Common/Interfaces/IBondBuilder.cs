using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.BondAggreagte;
using Domain.BondAggreagte.ValueObjects;

namespace Application.Calculation.Common.Interfaces;

public interface IBondBuilder
{
    public Task<IEnumerable<Bond>> BuildAsync(IEnumerable<Ticker> tickers, CancellationToken token = default);
    public Task<Bond> BuildAsync(Ticker ticker, CancellationToken token = default);
}