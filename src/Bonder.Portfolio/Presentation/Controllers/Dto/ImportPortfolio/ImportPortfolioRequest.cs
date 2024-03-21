using Domain.UserAggregate.ValueObjects.Portfolios;
using Microsoft.AspNetCore.Http;

namespace Presentation.Controllers.Dto.ImportPortfolio;
public sealed record ImportPortfolioRequest(Guid UserId, IFormFileCollection Files, BrokerType BrokerType, string? Name);