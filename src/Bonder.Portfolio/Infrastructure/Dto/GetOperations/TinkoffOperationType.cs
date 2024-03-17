namespace Infrastructure.Dto.GetOperations;

public static class TinkoffOperationType
{
    public const string Tax = "OPERATION_TYPE_TAX";
    public const string BondTax = "OPERATION_TYPE_BOND_TAX";

    public const string Input = "OPERATION_TYPE_INPUT";
    public const string Output = "OPERATION_TYPE_OUTPUT";

    public const string Sell = "OPERATION_TYPE_SELL";
    public const string Buy = "OPERATION_TYPE_BUY";

    public const string CouponInput = "OPERATION_TYPE_COUPON";

    public const string BrokerFee = "OPERATION_TYPE_BROKER_FEE";

    public const string MoneyTransfer = "OPERATION_TYPE_INP_MULTI";
    public const string AssetsTransfer = "OPERATION_TYPE_TRANS_IIS_BS";
}