namespace Domain.BondAggreagte.ValueObjects;
public readonly record struct BondId(Guid InstrumentId, Ticker Ticker, Isin Isin);
