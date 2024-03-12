using Domain.UserAggregate.ValueObjects.Trades;

namespace Domain.UserAggregate.ValueObjects.Operations;

public readonly record struct Operation(string Name,
                                        string Description,
                                        OperationType Type,
                                        OperationState State,
                                        DateTime Date,
                                        decimal Payout,
                                        decimal Price,
                                        decimal Commisison,
                                        InstrumentType InstrumentType,
                                        Guid? InstrumentId,
                                        int Quantity,
                                        int RestQuantity,
                                        IReadOnlyList<Trade> Trades);