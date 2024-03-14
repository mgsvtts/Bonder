using Application.Commands.AttachTinkoffToken;
using Application.Commands.ImportPortfolio;
using Application.Queries.GetOperations;
using Application.Queries.GetPortfolios;
using Domain.UserAggregate.Entities;
using Domain.UserAggregate.ValueObjects.Portfolios;
using Domain.UserAggregate.ValueObjects.Users;
using Mapster;
using Mediator;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Presentation.Controllers.Dto.AttachToken;
using Presentation.Controllers.Dto.ImportPortfolio;
using Presentation.Filters;
using Shared.Domain.Common;

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
    public async Task<IEnumerable<Portfolio>> GetPortfolios(CancellationToken token)
    {
        return await _sender.Send(new GetPortfoliosQuery(HttpContext.Request.Headers.Authorization), token);
    }

    [HttpPost("refresh")]
    public async Task<IResult> AttachToken([FromBody] AttachTokenRequest request, CancellationToken token)
    {
        await _sender.Send(request.Adapt<AttachTinkoffTokenCommand>(), token);

        return TypedResults.NoContent();
    }

    [HttpPost("import")]
    public async Task<IResult> Export(ImportPortfolioRequest request, CancellationToken token)
    {
        using var stream = request.File.OpenReadStream();

        await _sender.Send(new ImportPortfolioCommand(new UserId(request.UserId), stream, request.BrokerType, request.Name), token);

        return TypedResults.Created();
    }


    [HttpGet("{portfolioId:guid}/{currentPage:int?}")]
    public async Task<GetOperationsResponse> GetOperations([FromRoute] Guid portfolioId, int currentPage = 1, CancellationToken token = default)
    {
        return await _sender.Send(new GetOperationsQuery(new PortfolioId(portfolioId), new PageInfo(currentPage)), token);
    }
}