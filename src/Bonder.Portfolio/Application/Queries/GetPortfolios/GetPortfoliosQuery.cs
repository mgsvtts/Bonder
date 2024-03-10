using Domain.UserAggregate.Entities;
using Mediator;

namespace Application.Queries.GetPortfolios;
public sealed record GetPortfoliosQuery(string? IdentityToken) : IQuery<IEnumerable<Portfolio>>;