using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers.Portfolios.Dto.RefreshPortfolio;

public sealed record RefreshPortfolioRequest(string TinkoffToken);