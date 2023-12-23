using Application.Calculation.CalculateFigis;
using Application.Calculation.CalculateTickers;
using Application.Calculation.CalculateUids;
using Domain.BondAggreagte.ValueObjects;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Presentation.Controllers.BondController.Calculate;

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
    public async Task<IActionResult> CalculateTickers([FromBody] CalculateRequest request, CancellationToken token)
    {
        var result = request.IdType switch
        {
            IdType.Ticker => await _sender.Send(new CalculateTickersCommand(request.Ids.Select(x => new Ticker(x))), token),
            IdType.UID => await _sender.Send(new CalculateUidsCommand(request.Ids.Select(x => Guid.Parse(x.Trim()))), token),
            IdType.FIGI => await _sender.Send(new CalculateFigisCommand(request.Ids.Select(x => new Figi(x))), token),
            _ => throw new NotImplementedException(),
        };

        return Ok(_mapper.Map<CalculateJsonResponse>(result));
    }
}