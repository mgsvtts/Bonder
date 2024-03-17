namespace Domain.UserAggregate.ValueObjects.Operations;

public enum OperationType
{
    Unknown = 0,
    CouponInput = 1,
    Input = 2,
    Output = 3,
    BondTax = 4,
    Tax = 5,
    Sell = 6,
    BrokerFee = 7,
    Buy = 8,
    MoneyTransfer = 9,
    AssetsTransfer = 10
}