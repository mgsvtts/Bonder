using Domain.BondAggreagte.Abstractions;
using Domain.BondAggreagte.Abstractions.Dto.CalculateAll;
using Domain.BondAggreagte.Abstractions.Dto.GetPriceSorted;
using Domain.BondAggreagte.ValueObjects.Identities;
using Mediator;
using System.Collections.Generic;

namespace Application.Commands.Calculation.CalculateTickers;

public sealed class CalculateBondsByIdsCommandHandler : ICommandHandler<CalculateBondsByIdsCommand, CalculateBondsByIdsResponse>
{
    private readonly IBondRepository _repository;
    private readonly ICalculationService _calculator;

    public CalculateBondsByIdsCommandHandler(ICalculationService calculator, IBondRepository repository)
    {
        _calculator = calculator;
        _repository = repository;
    }

    public async ValueTask<CalculateBondsByIdsResponse> Handle(CalculateBondsByIdsCommand request, CancellationToken token)
    {
        GetPriceSortedResponse bonds;
        var notFound = new List<string>();

        if (request.IdType == IdType.Ticker)
        {
            bonds = await _repository.GetPriceSortedAsync(request.Options, tickers: request.Ids.Select(x => new Ticker(x)), token: token);

            notFound.AddRange(request.Ids.Except(bonds.Bonds.Select(x => x.Identity.Ticker.ToString())));
        }
        else
        {
            bonds = await _repository.GetPriceSortedAsync(request.Options, uids: request.Ids.Select(Guid.Parse), token: token);
            notFound.AddRange(request.Ids.Except(bonds.Bonds.Select(x => x.Identity.InstrumentId.ToString())));
        }

        return new CalculateBondsByIdsResponse(new CalculateAllResponse(_calculator.Calculate(new CalculationRequest(request.Options, bonds.Bonds)),
                                               bonds.PageInfo.Value),
                                               notFound);
    }
}
