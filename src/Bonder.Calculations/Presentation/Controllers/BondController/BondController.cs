using Application.Calculation.CalculateAll.Command;
using Application.Calculation.CalculateAll.Stream;
using Application.Calculation.CalculateTickers;
using Domain.BondAggreagte.Abstractions.Dto;
using Domain.BondAggreagte.Dto;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Presentation.Controllers.BondController.Calculate.Request;
using Presentation.Controllers.BondController.Calculate.Response;
using Presentation.Filters;
using System.Runtime.CompilerServices;

namespace Presentation.Controllers.BondController;

[ExceptionFilter]
[Route("api/calculate")]
public sealed class BondController : ControllerBase
{
    private readonly ISender _sender;
    private readonly IMapper _mapper;

    public BondController(ISender sender, IMapper mapper)
    {
        _sender = sender;
        _mapper = mapper;
    }

    [HttpPost]
    public async Task<CalculateResponse> Calculate([FromBody] CalculateBondsRequest request, CancellationToken token)
    {
        var result = await _sender.Send(_mapper.Map<CalculateBondsCommand>(request), token);

        return _mapper.Map<CalculateResponse>(result);
    }

    [HttpGet]
    public async IAsyncEnumerable<CalculateResponse> GetState([FromQuery] PageInfoRequest pageInfo, [EnumeratorCancellation] CancellationToken token)
    {
        var waitSeconds = TimeSpan.FromSeconds(5);
        await foreach (var result in _sender.CreateStream(new CalculateAllStreamRequest(new GetPriceSortedRequest(DateIntervalType.TillOfferDate,
                                                                                        new PageInfo(pageInfo.CurrentPage, pageInfo.ItemsOnPage))), token))
        {
            yield return _mapper.Map<CalculateResponse>(result);

            await Task.Delay(waitSeconds, token);
        }
    }

    [HttpGet("current-state")]
    public async Task<IActionResult> GetCurrentState(CalculationOptions request, CancellationToken token)
    {
        var result = await _sender.Send(new CalculateAllCommand(_mapper.Map<GetPriceSortedRequest>(request)), token);

        return Ok(_mapper.Map<CalculateResponse>(result));
    }
}