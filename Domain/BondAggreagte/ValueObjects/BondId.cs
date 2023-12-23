using Domain.BondAggreagte.Exceptions;

namespace Domain.BondAggreagte.ValueObjects;
public readonly record struct BondId
{
    public readonly Figi? Figi { get; }
    public readonly Ticker Ticker { get; }
    public readonly Guid Uid { get; }

    public BondId(Guid uid, Ticker ticker, Figi? figi = null)
    {
        if (ticker == default)
        {
            throw new DefaultTickerException();
        }
        if (figi != null && figi == default)
        {
            throw new DefaultFigiException();
        }
        if (uid == Guid.Empty)
        {
            throw new DefaultUidException();
        }

        Uid = uid;
        Figi = figi;
        Ticker = ticker;
    }
}