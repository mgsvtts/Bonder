using Domain.UserAggregate.Entities;
using Domain.UserAggregate.ValueObjects.Users;
using Mediator;

namespace Application.Queries.GetPortfolios;
public sealed record GetPortfoliosQuery(UserId UserId) : IQuery<IEnumerable<Portfolio>>;