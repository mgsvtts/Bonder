namespace Infrastructure.Dto.GetOperations;

public static class TinkoffOperationState
{
    public static string Executed => "OPERATION_STATE_EXECUTED";
    public static string InProgress => "OPERATION_STATE_PROGRESS";
    public static string Canceled => "OPERATION_STATE_CANCELED";
}