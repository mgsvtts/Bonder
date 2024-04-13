using Application.Queries.Common;
using Domain.BondAggreagte.Abstractions;
using Infrastructure.Common;
using LinqToDB;
using Mapster;
using Mediator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Queries.GetBondsByIds;
public sealed class GetBondsByIdsQueryHandler : IQueryHandler<GetBondsByIdsQuery, IEnumerable<BondItem>>
{
    public async ValueTask<IEnumerable<BondItem>> Handle(GetBondsByIdsQuery query, CancellationToken cancellationToken)
    {
        using var db = new DbConnection();

        var bonds = await db.Bonds
            .Where(x => query.Ids.Contains(x.Id))
            .ToListAsync(cancellationToken);

        return bonds.Adapt<IEnumerable<BondItem>>();
    }
}
