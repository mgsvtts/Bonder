using Application.Calculation.CalculateAll;
using Application.Calculation.CalculateTickers;
using Domain.BondAggreagte.Dto;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Presentation.Controllers.BondController.Calculate;
using Presentation.Controllers.BondController.Calculate.Request;
using Presentation.Controllers.BondController.Calculate.Response;
using System.Runtime.CompilerServices;

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
        await foreach (var result in _sender.CreateStream(new CalculateAllStreamRequest(new GetIncomeRequest(DateIntervalType.TillDate, DateTime.Now.AddYears(2))), token))
        {
            yield return result.MapToResponse();
        }
    }
}