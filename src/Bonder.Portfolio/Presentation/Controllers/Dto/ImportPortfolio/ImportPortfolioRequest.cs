using Domain.UserAggregate.ValueObjects.Portfolios;
using Microsoft.AspNetCore.Http;

namespace Presentation.Controllers.Dto.ImportPortfolio;
public sealed record ImportPortfolioRequest(Guid UserId, IFormFile File, BrokerType BrokerType);