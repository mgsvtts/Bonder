using Domain.UserAggregate.ValueObjects.Portfolios;
using Mediator;
using Shared.Domain.Common;

namespace Application.Queries.GetOperations;
public sealed record GetOperationsQuery(PortfolioId PortfolioId, PageInfo PageInfo) : IRequest<GetOperationsResponse>;