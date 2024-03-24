using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers.Dto.RefreshPortfolio;

public sealed record RefreshPortfolioRequest(string TinkoffToken);