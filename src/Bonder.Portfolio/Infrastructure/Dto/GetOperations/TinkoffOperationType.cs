namespace Infrastructure.Dto.GetOperations;

public static class TinkoffOperationType
{
    public static string Input => "OPERATION_TYPE_INPUT";
    public static string Output => "OPERATION_TYPE_OUTPUT";
    public static string Tax => "OPERATION_TYPE_BOND_TAX";
    public static string CouponInput => "OPERATION_TYPE_COUPON";
    public static string BrokerFee => "OPERATION_TYPE_BROKER_FEE";
    public static string Sell => "OPERATION_TYPE_SELL";
}