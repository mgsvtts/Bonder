﻿using Application.Commands.Calculation.CalculateAll.Command;
using Application.Commands.Calculation.CalculateAll.Stream;
using Application.Commands.Calculation.CalculateTickers;
using Domain.BondAggreagte.Abstractions.Dto.GetPriceSorted;
using Domain.BondAggreagte.Dto;
using Mapster;
using Mediator;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.AspNetCore.Routing;
using Presentation.Controllers.BondController.Calculate.Request;
using Presentation.Controllers.BondController.Calculate.Response;
using Presentation.Controllers.BondController.CalculateByIds;
using Presentation.Filters;
using Shared.Domain.Common;
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
    public async Task<CalculateByIdsResponse> Calculate([FromBody] CalculateBondsRequest request, CancellationToken token)
    {
        var result = await _sender.Send(request.Adapt<CalculateBondsByIdsCommand>(), token);

        return result.Adapt<CalculateByIdsResponse>();
    }

    [HttpGet("{currentPage:int?}")]
    public async IAsyncEnumerable<CalculateResponse> GetState([FromRoute] int currentPage = 1, [EnumeratorCancellation] CancellationToken token = default)
    {
        var waitSeconds = TimeSpan.FromSeconds(5);
        await foreach (var result in _sender.CreateStream(new CalculateAllStreamCommand(new GetPriceSortedRequest(DateIntervalType.TillOfferDate,
                                                                                        new PageInfo(currentPage))), token))
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