using Domain.UserAggregate.Abstractions.Repositories;
using Domain.UserAggregate.ValueObjects.Operations;
using Mapster;
using Mediator;

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
