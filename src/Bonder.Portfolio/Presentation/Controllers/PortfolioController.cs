using Application.AttachTinkoffToken;
using Application.GetPortfolios;
using Domain.UserAggregate.ValueObjects.Portfolios;
using Mapster;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Presentation.Controllers.Dto.AttachToken;
using Presentation.Filters;

namespace Presentation.Controllers;

[ExceptionFilter]
[Route("api/portfolio")]
public sealed class PortfolioController : ControllerBase
{
    private readonly ISender _sender;

    public PortfolioController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost("attach-token")]
    public async Task<IActionResult> AttachToken([FromBody] AttachTokenRequest request, CancellationToken cancellationToken)
    {
        await _sender.Send(request.Adapt<AttachTinkoffTokenCommand>(), cancellationToken);

        return NoContent();
    }

    [HttpGet]
    public async Task<IEnumerable<Portfolio>> GetPortfolios(CancellationToken cancellationToken)
    {
        return await _sender.Send(new GetPortfoliosQuery(HttpContext.Request.Headers.Authorization), cancellationToken);
    }
}