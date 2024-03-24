using Application.Commands.Analyze;
using Mapster;
using Mediator;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Presentation.Controllers.AnalyzeController.Analyze;

namespace Presentation.Controllers.AnalyzeController;

[Route("api/advice")]
public sealed class AdviceController : ControllerBase
{
    private readonly ISender _sender;

    public AdviceController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost]
    [OutputCache(Duration = 10)]
    public async Task<IEnumerable<AnalyzeBondsResponse>> Analyze([FromBody] AnalyzeBondsRequest request, CancellationToken token)
    {
        var result = await _sender.Send(request.Adapt<AdviceBondsCommand>(), token);

        return result.Adapt<IEnumerable<AnalyzeBondsResponse>>();
    }
}