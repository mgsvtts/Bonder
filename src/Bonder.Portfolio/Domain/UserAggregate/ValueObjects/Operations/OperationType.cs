namespace Domain.UserAggregate.ValueObjects.Operations;

public enum OperationType
{
    /// <summary>
    /// Незвестный тип
    /// </summary>
    Unknown = 0,

    /// <summary>
    /// Выплата купонов
    /// </summary>
    CouponInput = 1,

    /// <summary>
    /// Пополнение брокерского счёта
    /// </summary>
    Input = 2,

    /// <summary>
    /// Вывод денежных средств
    /// </summary>
    Output = 3,

    /// <summary>
    /// Удержание налога по купонам
    /// </summary>
    BondTax = 4,

    /// <summary>
    /// Удержание налога
    /// </summary>
    Tax = 5,

    /// <summary>
    /// Продажа
    /// </summary>
    Sell = 6,

    /// <summary>
    /// Комиссия брокера
    /// </summary>
    BrokerFee = 7,

    /// <summary>
    /// Покупка
    /// </summary>
    Buy = 8,

    /// <summary>
    /// Перевод денежный средств
    /// </summary>
    MoneyTransfer = 9,

    /// <summary>
    /// Перевод активов
    /// </summary>
    AssetsTransfer = 10
}