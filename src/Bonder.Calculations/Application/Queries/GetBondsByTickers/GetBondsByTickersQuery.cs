using Domain.BondAggreagte.ValueObjects.Identities;
using Mediator;

namespace Application.Queries.GetBondsByTickers;
public sealed record GetBondsByTickersQuery(IEnumerable<Ticker> Tickers) : IQuery<IEnumerable<BondItem>>;