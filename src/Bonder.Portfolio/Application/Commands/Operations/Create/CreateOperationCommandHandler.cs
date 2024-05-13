using Domain.UserAggregate.Abstractions.Repositories;
using Domain.UserAggregate.ValueObjects.Operations;
using Domain.UserAggregate.ValueObjects.Trades;
using Infrastructure;
using Mapster;
using Mediator;
using Shared.Domain.Common.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Commands.Operations.Create;
public sealed class CreateOperationCommandHandler : IRequestHandler<CreateOperationCommand>
{
    private readonly IPortfolioRepository _portfolioRepository;

    public CreateOperationCommandHandler(IPortfolioRepository portfolioRepository)
    {
        _portfolioRepository = portfolioRepository;
    }

    public async ValueTask<Unit> Handle(CreateOperationCommand request, CancellationToken cancellationToken)
    {
        var operation = request.Adapt<Operation>();

        await _portfolioRepository.AddOperation(request.PortfolioId, operation, cancellationToken);
    
        return Unit.Value;
    }
} 
