using Domain.UserAggregate.ValueObjects.Operations;
using LinqToDB.Mapping;

namespace Infrastructure.Common.Models;

[Table("operations")]
public sealed class Operation
{
    [Column("id")]
    public Guid Id { get; set; }

    [Column("name")]
    public string Name { get; set; }

    [Column("description")]
    public string Description { get; set; }

    [Column("portfolio_id")]
    public Guid PortfolioId { get; set; }

    [Column("type")]
    public OperationType Type { get; set; }

    [Column("state")]
    public OperationState State { get; set; }

    [Column("instrument_id")]
    public Guid? InstrumentId { get; set; }

    [Column("instrument_type")]
    public InstrumentType InstrumentType { get; set; }

    [Column("quantity")]
    public int Quantity { get; set; }

    [Column("rest_quantity")]
    public int RestQuantity { get; set; }

    [Column("date")]
    public DateTime Date { get; set; }

    [Column("payout")]
    public decimal Payout { get; set; }

    [Column("price")]
    public decimal Price { get; set; }

    [Column("commission")]
    public decimal Commission { get; set; }

    [Association(ThisKey = nameof(Id), OtherKey = nameof(Trade.OperationId))]
    public List<Trade> Trades { get; set; }
}