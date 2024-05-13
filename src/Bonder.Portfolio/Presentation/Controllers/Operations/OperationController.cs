using Application.Commands.Operations.Create;
using Application.Queries.GetOperations;
using Domain.UserAggregate.ValueObjects.Portfolios;
using Mapster;
using Mediator;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Presentation.Controllers.Operations.Dto.Create;
using Presentation.Filters;
using Shared.Domain.Common;

namespace Presentation.Controllers.Operations;

[ExceptionFilter]
[Route("api/operation")]
public sealed class OperationController : ControllerBase
{
    private const int _cacheTime = 10;

    private readonly ISender _sender;

    public OperationController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost]
    public async Task CreateOperationAsync(CreateOperationRequest request, CancellationToken token)
    {
        await _sender.Send(request.Adapt<CreateOperationCommand>(), token);
    }

    [HttpGet("{portfolioId:guid}/{currentPage:int?}")]
    [OutputCache(Duration = _cacheTime)]
    public async Task<GetOperationsResponse> GetOperationsAsync([FromRoute] Guid portfolioId, int currentPage = 1, CancellationToken token = default)
    {
        return await _sender.Send(new GetOperationsQuery(new PortfolioId(portfolioId), new PageInfo(currentPage)), token);
    }
}
