using Domain.BondAggreagte.Abstractions;
using Domain.BondAggreagte.Abstractions.Dto.CalculateAll;
using Domain.BondAggreagte.Abstractions.Dto.GetPriceSorted;
using Domain.BondAggreagte.ValueObjects.Identities;
using Mediator;

namespace Application.Commands.Calculation.CalculateTickers;

public sealed class CalculateBondsCommandHandler : ICommandHandler<CalculateBondsCommand, CalculateAllResponse>
{
    private readonly IBondRepository _repository;
    private readonly ICalculationService _calculator;

    public CalculateBondsCommandHandler(ICalculationService calculator, IBondRepository repository)
    {
        _calculator = calculator;
        _repository = repository;
    }

    public async ValueTask<CalculateAllResponse> Handle(CalculateBondsCommand request, CancellationToken token)
    {
        GetPriceSortedResponse bonds;
        if (request.IdType == IdType.Ticker)
        {
            bonds = await _repository.GetPriceSortedAsync(request.Options, tickers: request.Ids.Select(x => new Ticker(x)), token: token);
        }
        else
        {
            bonds = await _repository.GetPriceSortedAsync(request.Options, uids: request.Ids.Select(Guid.Parse), token: token);
        }

        return new CalculateAllResponse(_calculator.Calculate(new CalculationRequest(request.Options, bonds.Bonds)), bonds.PageInfo.Value);
    }
}