using Application.Calculation.Common.Interfaces;
using Google.Protobuf.WellKnownTypes;
using LinqToDB;
using MapsterMapper;
using Tinkoff.InvestApi;
using Tinkoff.InvestApi.V1;

namespace Infrastructure.Calculation.Common;

public class TinkoffGrpcClient : ITinkoffGrpcClient
{
    private readonly InvestApiClient _tinkoffApiClient;
    private readonly IMapper _mapper;

    public TinkoffGrpcClient(InvestApiClient client, IMapper mapper)
    {
        _tinkoffApiClient = client;
        _mapper = mapper;
    }

    public async Task<List<Domain.BondAggreagte.ValueObjects.Coupon>> GetCouponsAsync(Guid instrumentId, CancellationToken token = default)
    {
        var coupons = await _tinkoffApiClient.Instruments.GetBondCouponsAsync(new GetBondCouponsRequest
        {
            InstrumentId = instrumentId.ToString(),
            From = Timestamp.FromDateTime(DateTime.MinValue.ToUniversalTime()),
            To = Timestamp.FromDateTime(DateTime.MaxValue.ToUniversalTime())
        }, cancellationToken: token);

        return _mapper.Map<List<Domain.BondAggreagte.ValueObjects.Coupon>>(coupons.Events);
    }
}