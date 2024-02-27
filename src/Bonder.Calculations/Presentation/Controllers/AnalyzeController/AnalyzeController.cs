using Application.Analyze;
using Mapster;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Presentation.Controllers.AnalyzeController.Analyze;

namespace Presentation.Controllers.AnalyzeController;

[Route("api/analyze")]
public sealed class AnalyzeController : ControllerBase
{
    private readonly ISender _sender;

    public AnalyzeController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost]
    public async Task<IEnumerable<AnalyzeBondsResponse>> Analyze([FromBody] AnalyzeBondsRequest request, CancellationToken token)
    {
        var result = await _sender.Send(request.Adapt<AnalyzeBondsCommand>(), token);

        return result.Adapt<IEnumerable<AnalyzeBondsResponse>>();
    }
}