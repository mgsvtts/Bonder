using Domain.UserAggregate.ValueObjects.Operations;
using Domain.UserAggregate.ValueObjects.Portfolios;
using Mediator;
using Shared.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Queries.GetOperations;
public sealed record GetOperationsQuery(PortfolioId PortfolioId, PageInfo PageInfo) : IRequest<GetOperationsResponse>;