namespace Domain.UserAggregate.ValueObjects.Operations;

public enum OperationState
{
    Unknown,
    Executed,
    Canceled,
    InProgress
}