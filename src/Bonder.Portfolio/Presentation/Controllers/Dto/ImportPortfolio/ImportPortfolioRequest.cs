using Domain.UserAggregate.ValueObjects.Portfolios;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers.Dto.ImportPortfolio;
public sealed record ImportPortfolioRequest([FromForm] IFormFileCollection Files,
                                            [FromForm] BrokerType BrokerType,
                                            [FromForm] string? Name,
                                            [FromHeader(Name = "X-USER-ID")] Guid userId);