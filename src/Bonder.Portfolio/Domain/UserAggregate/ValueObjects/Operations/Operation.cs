namespace Domain.UserAggregate.ValueObjects.Operations;

public readonly record struct Operation(OperationType Type, OperationState State, DateTime Date, decimal Payout);
