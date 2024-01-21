﻿using Application.Calculation.CalculateAll;
using Application.Calculation.CalculateTickers;
using Domain.BondAggreagte.Dto;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Presentation.Controllers.BondController.Calculate;
using Presentation.Controllers.BondController.Calculate.Request;
using Presentation.Controllers.BondController.Calculate.Response;
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace Presentation.Controllers.BondController;

[Route("api/calculate")]
public class BondController : ControllerBase
{
    private readonly ISender _sender;
    private readonly IMapper _mapper;

    public BondController(ISender sender, IMapper mapper)
    {
        _sender = sender;
        _mapper = mapper;
    }

    [HttpPost]
    public async Task<CalculateResponse> CalculateTickers([FromBody] CalculateRequest request, CancellationToken token)
    {
        var result = await _sender.Send(_mapper.Map<CalculateTickersCommand>(request), token);

        return result.MapToResponse();
    }

    [HttpGet]
    public async IAsyncEnumerable<CalculateResponse> GetState([EnumeratorCancellation] CancellationToken token)
    {
        var waitSeconds = TimeSpan.FromSeconds(5);
        await foreach (var result in _sender.CreateStream(new CalculateAllStreamRequest(new GetIncomeRequest(DateIntervalType.TillOfferDate)), token))
        {
            yield return result.MapToResponse();

            await Task.Delay(waitSeconds, token);
        }
    }
}