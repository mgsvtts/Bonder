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

    public async ValueTask<IEnumerable<BondItem>> Handle(GetBondsByTickersQuery query, CancellationToken cancellationToken)
    {
        var bonds = await _bondRepository.GetByTickersAsync(query.Tickers, cancellationToken);

        return bonds.Adapt<IEnumerable<BondItem>>();
    }
}