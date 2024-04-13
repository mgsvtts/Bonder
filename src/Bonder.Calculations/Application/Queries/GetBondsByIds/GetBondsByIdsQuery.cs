using Application.Queries.Common;
using Domain.BondAggreagte.ValueObjects.Identities;
using Mediator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Queries.GetBondsByIds;
public sealed record GetBondsByIdsQuery(IEnumerable<Guid> Ids) : IQuery<IEnumerable<BondItem>>;
