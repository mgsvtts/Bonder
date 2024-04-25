using Application.Commands.Calculation.CalculateAll.Command;
using Application.Queries.GetBondsByIds;
using Application.Queries.GetBondsByTickers;
using Bonder.Calculation.Grpc;
using Domain.BondAggreagte.Abstractions.Dto.GetPriceSorted;
using Domain.BondAggreagte.Dto;
using Domain.BondAggreagte.ValueObjects.Identities;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Mapster;
using Mediator;
using Shared.Domain.Common;

namespace Presentation.Grpc;

public sealed class CalculationServiceImpl : CalculationService.CalculationServiceBase
{
    private readonly ISender _sender;

    public CalculationServiceImpl(ISender sender)
    {
        _sender = sender;
    }

    public override async Task<BondsResponse> GetBondsByTickers(GetBondsByTickersRequest request, ServerCallContext context)
    {
        var bonds = await _sender.Send(new GetBondsByTickersQuery(request.Tickers.Select(x => new Ticker(x))), context.CancellationToken);

        return bonds.Adapt<BondsResponse>();
    }

    public override async Task<GetCurrentBondsResponse> GetCurrentBonds(Bonder.Calculation.Grpc.Filters request, ServerCallContext context)
    {
        var bonds = await _sender.Send(request.Adapt<CalculateAllCommand>(), context.CancellationToken);

        return bonds.Adapt<GetCurrentBondsResponse>();
    }
}