namespace Domain.UserAggregate.ValueObjects.Operations;

public enum OperationState
{
    Unknown = 0,
    Executed = 1,
    Canceled = 2,
    InProgress = 3
}