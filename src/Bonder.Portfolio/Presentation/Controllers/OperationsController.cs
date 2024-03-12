using Mediator;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers;

public sealed class OperationsController : ControllerBase
{
    private readonly ISender _sender;

    public OperationsController(ISender sender)
    {
        _sender = sender;
    }

    // public async Task<IEnumerable<Operation>> GetOperations(Guid portfolioId)
}