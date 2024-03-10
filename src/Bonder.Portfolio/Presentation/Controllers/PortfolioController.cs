using Application.Commands.AttachTinkoffToken;
using Application.Commands.ImportPortfolio;
using Application.Queries.GetPortfolios;
using Domain.UserAggregate.Entities;
using Domain.UserAggregate.ValueObjects;
using Domain.UserAggregate.ValueObjects.Users;
using Mapster;
using Mediator;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Presentation.Controllers.Dto.AttachToken;
using Presentation.Controllers.Dto.ImportPortfolio;
using Presentation.Filters;

namespace Presentation.Controllers;

[ExceptionFilter]
[Route("api/portfolio")]
public sealed class PortfolioController : ControllerBase
{
    private const int _cacheTime = 10;
    private readonly ISender _sender;

    public PortfolioController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet]
    [OutputCache(Duration = _cacheTime)]
    public async Task<IEnumerable<Portfolio>> GetPortfolios(CancellationToken cancellationToken)
    {
        return await _sender.Send(new GetPortfoliosQuery(HttpContext.Request.Headers.Authorization), cancellationToken);
    }

    [HttpPost("attach-token")]
    public async Task<IResult> AttachToken([FromBody] AttachTokenRequest request, CancellationToken cancellationToken)
    {
        await _sender.Send(request.Adapt<AttachTinkoffTokenCommand>(), cancellationToken);

        return TypedResults.NoContent();
    }

    [HttpPost("import")]
    public async Task<IResult> ExportAsync(ImportPortfolioRequest request)
    {
        using var stream = request.File.OpenReadStream();

        await _sender.Send(new ImportPortfolioCommand(new UserId(request.UserId), stream, request.BrokerType, request.Name));

        return TypedResults.Created();
    }
}