namespace Domain.BondAggreagte.ValueObjects;
public readonly record struct BondId(Guid Id, Ticker Ticker, Isin Isin);
