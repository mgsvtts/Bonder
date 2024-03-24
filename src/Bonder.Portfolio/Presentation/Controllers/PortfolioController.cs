using Application.Commands.ImportPortfolio;
using Application.Commands.RefreshPortfolio;
using Application.Queries.GetOperations;
using Application.Queries.GetPortfolios;
using Application.Queries.GetStats;
using Domain.UserAggregate.Entities;
using Domain.UserAggregate.ValueObjects.Portfolios;
using Domain.UserAggregate.ValueObjects.Users;
using Mapster;
using Mediator;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Presentation.Controllers.Dto.GetStats;
using Presentation.Controllers.Dto.ImportPortfolio;
using Presentation.Controllers.Dto.RefreshPortfolio;
using Presentation.Extensions;
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
    public async Task<IEnumerable<Portfolio>> GetPortfoliosAsync([FromHeader(Name = "X-USER-ID")] Guid userId, CancellationToken token)
    {
        return await _sender.Send(new GetPortfoliosQuery(new UserId(userId)), token);
    }

    [HttpPost("refresh")]
    public async Task<IResult> AttachTokenAsync([FromBody]RefreshPortfolioRequest request, [FromHeader(Name = "X-USER-ID")] Guid userId, CancellationToken token)
    {
        await _sender.Send((request, userId).Adapt<RefreshPortfolioCommand>(), token);

        return TypedResults.NoContent();
    }

    [HttpPost("import")]
    public async Task<IResult> ImportAsync(ImportPortfolioRequest request, CancellationToken token)
    {
        var streams = request.Files.OpenReadStreams();

        await _sender.Send(new ImportPortfolioCommand(new UserId(request.UserId), request.BrokerType, request.Name, streams), token);

        streams.Dispose();

        return TypedResults.Created();
    }

    [HttpGet("stats")]
    [OutputCache(Duration = _cacheTime)]
    public async Task<GetStatsResult> PortfolioStatsAsync([FromQuery]GetStatsRequest request, [FromHeader(Name = "X-USER-ID")] Guid currentUserId, CancellationToken token)
    {
        return await _sender.Send((request, currentUserId).Adapt<GetStatsQuery>(), token);
    }

    [HttpGet("{portfolioId:guid}/{currentPage:int?}")]
    [OutputCache(Duration = _cacheTime)]
    public async Task<GetOperationsResponse> GetOperationsAsync([FromRoute] Guid portfolioId, int currentPage = 1, CancellationToken token = default)
    {
        return await _sender.Send(new GetOperationsQuery(new PortfolioId(portfolioId), new PageInfo(currentPage)), token);
    }
}
