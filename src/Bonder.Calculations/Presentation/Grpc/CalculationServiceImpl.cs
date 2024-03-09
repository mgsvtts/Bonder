using Application.Queries.GetBondsByTickers;
using Bonder.Calculation.Grpc;
using Domain.BondAggreagte.ValueObjects.Identities;
using Grpc.Core;
using Mapster;
using Mediator;

namespace Presentation.Grpc;

public sealed class CalculationServiceImpl : CalculationService.CalculationServiceBase
{
    private readonly ISender _sender;

    public CalculationServiceImpl(ISender sender)
    {
        _sender = sender;
    }

    public override async Task<GetBondsByTickersResponse> GetBondsByTickers(GetBondsByTickersRequest request, ServerCallContext context)
    {
        var bonds = await _sender.Send(new GetBondsByTickersQuery(request.Tickers.Select(x => new Ticker(x))), context.CancellationToken);

        return bonds.Adapt<GetBondsByTickersResponse>();
    }
}