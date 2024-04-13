using Application.Queries.Common;
using Domain.BondAggreagte.Abstractions;
using Mapster;
using Mediator;

namespace Application.Queries.GetBondsByTickers;

public sealed class GetBondsByTickersQueryHandler : IQueryHandler<GetBondsByTickersQuery, IEnumerable<BondItem>>
{
    private readonly IBondRepository _bondRepository;

    public GetBondsByTickersQueryHandler(IBondRepository bondRepository)
    {
        _bondRepository = bondRepository;
    }

    public async ValueTask<IEnumerable<BondItem>> Handle(GetBondsByTickersQuery query, CancellationToken token)
    {
        var bonds = await _bondRepository.GetByTickersAsync(query.Tickers, token);

        return bonds.Adapt<IEnumerable<BondItem>>();
    }
}