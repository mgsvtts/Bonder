using Domain.UserAggregate.ValueObjects;
using Domain.UserAggregate.ValueObjects.Portfolios;
using MediatR;

namespace Application.GetPortfolios;
public sealed record GetPortfoliosQuery(string? IdentityToken) : IRequest<IEnumerable<Portfolio>>;