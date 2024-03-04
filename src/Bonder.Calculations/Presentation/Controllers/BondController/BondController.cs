using Application.Calculation.CalculateAll.Command;
using Application.Calculation.CalculateAll.Stream;
using Application.Calculation.CalculateTickers;
using Domain.BondAggreagte.Abstractions.Dto;
using Domain.BondAggreagte.Dto;
using Mapster;
using Mediator;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Presentation.Controllers.BondController.Calculate.Request;
using Presentation.Controllers.BondController.Calculate.Response;
using Presentation.Filters;
using System.Runtime.CompilerServices;

namespace Presentation.Controllers.BondController;

[ExceptionFilter]
[Route("api/calculate")]
public sealed class BondController : ControllerBase
{
    private const int _cacheTime = 10;
    private readonly ISender _sender;

    public BondController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost]
    [OutputCache(Duration = _cacheTime)]
    public async Task<CalculateResponse> Calculate([FromBody] CalculateBondsRequest request, CancellationToken token)
    {
        var result = await _sender.Send(request.Adapt<CalculateBondsCommand>(), token);

        return result.Adapt<CalculateResponse>();
    }

    [HttpGet]
    public async IAsyncEnumerable<CalculateResponse> GetState([FromQuery] PageInfoRequest pageInfo, [EnumeratorCancellation] CancellationToken token)
    {
        var waitSeconds = TimeSpan.FromSeconds(5);
        await foreach (var result in _sender.CreateStream(new CalculateAllStreamCommand(new GetPriceSortedRequest(DateIntervalType.TillOfferDate,
                                                                                        new PageInfo(pageInfo.CurrentPage))), token))
        {
            yield return result.Adapt<CalculateResponse>();

            await Task.Delay(waitSeconds, token);
        }
    }

    [HttpGet("current")]
    [OutputCache(Duration = _cacheTime)]
    public async Task<CalculateResponse> GetCurrentState(CalculationOptions request, CancellationToken token)
    {
        var result = await _sender.Send(new CalculateAllCommand(request.Adapt<GetPriceSortedRequest>()), token);

        return result.Adapt<CalculateResponse>();
    }
}