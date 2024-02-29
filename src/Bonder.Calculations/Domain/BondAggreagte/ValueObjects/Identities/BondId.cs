namespace Domain.BondAggreagte.ValueObjects.Identities;
public readonly record struct BondId(Guid InstrumentId, Ticker Ticker, Isin Isin);