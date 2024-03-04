using Domain.UserAggregate.ValueObjects.Portfolios;
using Mediator;

namespace Application.GetPortfolios;
public sealed record GetPortfoliosQuery(string? IdentityToken) : IQuery<IEnumerable<Portfolio>>;