using Domain.BondAggreagte.Abstractions;
using Domain.BondAggreagte.ValueObjects;
using Domain.BondAggreagte.ValueObjects.Identities;
using Grpc.Core;
using Quartz;
using Serilog;

namespace Application.Commands.Calculation.CalculateAll.Jobs;

[DisallowConcurrentExecution]
public sealed class UpdateBondPriceJob : IJob
{
    private readonly IAllBondsReceiver _bondReceiver;
    private readonly IBondRepository _bondRepository;
    private readonly IBondBuilder _bondBuilder;

    public UpdateBondPriceJob(IAllBondsReceiver bondReceiver,
                              IBondRepository bondRepository,
                              IBondBuilder bondBuilder)
    {
        _bondReceiver = bondReceiver;
        _bondRepository = bondRepository;
        _bondBuilder = bondBuilder;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        try
        {
            var bondsToUpdate = await TryReceiveAsync(context.CancellationToken);

            await ProcessBondsAsync(bondsToUpdate, context.CancellationToken);
        }
        catch(Exception ex)
        {
            Log.Error(ex, "Error in UpdateBondPriceJob");
        }
    }

    private async Task<IEnumerable<KeyValuePair<Ticker, StaticIncome>>> TryReceiveAsync(CancellationToken token)
    {
        try
        {
            return await _bondReceiver.ReceiveAsync(token);
        }
        catch (RpcException ex)
        {
            Log.Error(ex, "RpcException in UpdateBondPriceJob, starting retries...");

            return await TryRetryAsync(token);
        }
    }

    private async Task ProcessBondsAsync(IEnumerable<KeyValuePair<Ticker, StaticIncome>> bondsToUpdate, CancellationToken token)
    {
        var notFoundTickers = await _bondRepository.UpdateIncomesAsync(bondsToUpdate, token);
        if (notFoundTickers.Count == 0)
        {
            return;
        }

        var notFoundBonds = await _bondBuilder.BuildAsync(notFoundTickers, token);

        await _bondRepository.AddAsync(notFoundBonds, token);
    }

    private async Task<IEnumerable<KeyValuePair<Ticker, StaticIncome>>> TryRetryAsync(CancellationToken token)
    {
        var retries = 0;
        while (retries <= 5)
        {
            try
            {
                return await _bondReceiver.ReceiveAsync(token);
            }
            catch
            {
                Log.Warning("Retry {retry} failed", retries);
            }

            retries++;
        }

        return Enumerable.Empty<KeyValuePair<Ticker, StaticIncome>>();
    }
}