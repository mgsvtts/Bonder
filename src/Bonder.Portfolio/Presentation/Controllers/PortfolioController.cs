using Application.Commands.ImportPortfolio;
using Application.Commands.RefreshPortfolio;
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
    public async Task<IEnumerable<Portfolio>> GetPortfoliosAsync([FromHeader(Name = "X-USER-ID")] Guid userId, CancellationToken token)
    {
        return await _sender.Send(new GetPortfoliosQuery(new UserId(userId)), token);
    }

    [HttpPost("refresh")]
    public async Task<IResult> AttachTokenAsync([FromBody] string tinkoffToken, [FromHeader(Name = "X-USER-ID")] Guid userId, CancellationToken token)
    {
        await _sender.Send((tinkoffToken, userId).Adapt<RefreshPortfolioCommand>(), token);

        return TypedResults.NoContent();
    }

    [HttpPost("import")]
    public async Task<IResult> ImportAsync(ImportPortfolioRequest request, CancellationToken token)
    {
        var streams = new List<Stream>();

        foreach (var file in request.Files)
        {
            streams.Add(file.OpenReadStream());
        }

        await _sender.Send(new ImportPortfolioCommand(new UserId(request.userId), request.BrokerType, request.Name, streams), token);

        foreach (var stream in streams)
        {
            stream.Dispose();
        }

        return TypedResults.Created();
    }


    [HttpGet("{portfolioId:guid}/{currentPage:int?}")]
    public async Task<GetOperationsResponse> GetOperationsAsync([FromRoute] Guid portfolioId, int currentPage = 1, CancellationToken token = default)
    {
        return await _sender.Send(new GetOperationsQuery(new PortfolioId(portfolioId), new PageInfo(currentPage)), token);
    }
}