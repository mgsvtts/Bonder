using Application.Commands.Calculation.Common.Abstractions;
using Google.Protobuf.WellKnownTypes;
using Mapster;
using Tinkoff.InvestApi;
using Tinkoff.InvestApi.V1;

namespace Infrastructure.Calculation.CalculateAll.GrpcClients;

public sealed class TinkoffGrpcClient : ITinkoffGrpcClient
{
    private readonly InvestApiClient _tinkoffApiClient;

    public TinkoffGrpcClient(InvestApiClient client)
    {
        _tinkoffApiClient = client;
    }

    public async Task<List<Domain.BondAggreagte.ValueObjects.Incomes.Coupon>> GetCouponsAsync(Guid instrumentId, CancellationToken token = default)
    {
        var coupons = await _tinkoffApiClient.Instruments.GetBondCouponsAsync(new GetBondCouponsRequest
        {
            InstrumentId = instrumentId.ToString(),
            From = Timestamp.FromDateTime(DateTime.MinValue.ToUniversalTime()),
            To = Timestamp.FromDateTime(DateTime.MaxValue.ToUniversalTime())
        }, cancellationToken: token);

        return coupons.Events.Adapt<List<Domain.BondAggreagte.ValueObjects.Incomes.Coupon>>();
    }
}