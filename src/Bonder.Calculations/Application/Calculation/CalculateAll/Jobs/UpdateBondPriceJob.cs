using Application.Calculation.Common.Abstractions;
using Domain.BondAggreagte.Abstractions;
using Domain.BondAggreagte.ValueObjects;
using Grpc.Core;
using Quartz;

namespace Application.Calculation.CalculateAll.Jobs;

[DisallowConcurrentExecution]
public class UpdateBondPriceJob : IJob
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
            await ExecuteAsync(context);
        }
        catch
        { }
    }

    private async Task ExecuteAsync(IJobExecutionContext context)
    {
        const int step = 10;
        var startRange = new Range(0, step);

        while (startRange.Start.Value - step <= _bondReceiver.GetMaxRange())
        {
            var bondsToUpdate = await TryReceiveAsync(startRange, context.CancellationToken);

            RecreateRange(step, ref startRange);

            await ProcessBondsAsync(bondsToUpdate, context.CancellationToken);
        }
    }

    private async Task<IEnumerable<KeyValuePair<Ticker, StaticIncome>>> TryReceiveAsync(Range startRange,
                                                                                         CancellationToken token)
    {
        try
        {
            return await _bondReceiver.ReceiveAsync(startRange, token);
        }
        catch (RpcException)
        {
            return await TryRetryAsync(startRange, token);
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

    private void RecreateRange(int step, ref Range startRange)
    {
        if (startRange.Start.Value >= _bondReceiver.GetMaxRange())
        {
            startRange = new Range(0, step);
        }
        else
        {
            startRange = new Range(startRange.End, startRange.End.Value + step);
        }
    }

    private async Task<IEnumerable<KeyValuePair<Ticker, StaticIncome>>> TryRetryAsync(Range start, CancellationToken token)
    {
        var retries = 5;
        while (retries > 0)
        {
            try
            {
                return await _bondReceiver.ReceiveAsync(start, token);
            }
            catch
            { }

            retries--;
        }

        return Array.Empty<KeyValuePair<Ticker, StaticIncome>>();
    }
}