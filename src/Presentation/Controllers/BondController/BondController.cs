using Application.Calculation.CalculateAll;
using Application.Calculation.CalculateTickers;
using Application.Calculation.Common.CalculationService.Dto;
using Application.Calculation.Common.Interfaces;
using Domain.BondAggreagte.Dto;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Presentation.Controllers.BondController.Calculate;
using Presentation.Controllers.BondController.Calculate.Request;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

namespace Presentation.Controllers.BondController;

[Route("api/calculate")]
public class BondController : ControllerBase
{
    private readonly ISender _sender;
    private readonly IMapper _mapper;
    private readonly ICalculator _calculator;
    public BondController(ISender sender, IMapper mapper, ICalculator calculator)
    {
        _sender = sender;
        _mapper = mapper;
        _calculator = calculator;
    }

    [HttpPost]
    public async Task<IActionResult> CalculateTickers([FromBody] CalculateRequest request, CancellationToken token)
    {
        var result = await _sender.Send(_mapper.Map<CalculateTickersCommand>(request), token);

        return Ok(result.MapToResponse());
    }

    [HttpGet]
    public async Task GetState(CancellationToken token)
    {
        if (!HttpContext.WebSockets.IsWebSocketRequest)
        {
            HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
        }

        using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
        while (webSocket.CloseStatus == null)
        {
            var bonds = _calculator.Calculate(new CalculationRequest(new GetIncomeRequest(DateIntervalType.TillDate, DateTime.Now.AddMonths(6)), AllBonds.State));
            var socketMessage = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(bonds.MapToResponse()));
            await webSocket.SendAsync(new ArraySegment<byte>(socketMessage, 0, socketMessage.Length), WebSocketMessageType.Text, true, token);
            await Task.Delay(TimeSpan.FromSeconds(5), token);
        }

        await webSocket.CloseAsync(webSocket.CloseStatus.Value, webSocket.CloseStatusDescription, token);
    }
}