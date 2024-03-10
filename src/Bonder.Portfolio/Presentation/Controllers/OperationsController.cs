using Domain.UserAggregate.ValueObjects.Operations;
using Mediator;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
