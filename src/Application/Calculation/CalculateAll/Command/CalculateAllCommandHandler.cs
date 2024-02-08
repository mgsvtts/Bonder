using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Calculation.CalculateAll.Query;
using Application.Calculation.CalculateAll.Services;
using Application.Calculation.Common.CalculationService.Dto;
using MediatR;

namespace Application.Calculation.CalculateAll.Command;

public class CalculateAllCommandHandler : IRequestHandler<CalculateAllCommand, CalculationResults>
{
    private readonly ICalculateAllService _service;

    public CalculateAllCommandHandler(ICalculateAllService service)
    {
        _service = service;
    }

    public async Task<CalculationResults> Handle(CalculateAllCommand request, CancellationToken cancellationToken)
    {
        return await _service.CalculateAllAsync(request.IncomeRequest, cancellationToken);
    }
}