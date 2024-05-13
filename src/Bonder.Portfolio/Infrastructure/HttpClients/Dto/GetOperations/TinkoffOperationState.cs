namespace Infrastructure.HttpClients.Dto.GetOperations;

public static class TinkoffOperationState
{
    public const string Executed = "OPERATION_STATE_EXECUTED";
    public const string InProgress = "OPERATION_STATE_PROGRESS";
    public const string Canceled = "OPERATION_STATE_CANCELED";
}