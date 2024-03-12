namespace Domain.UserAggregate.ValueObjects.Trades;

public readonly record struct Trade(DateTime Date,
                                    int Quantity,
                                    decimal Price);