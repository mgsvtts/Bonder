using Domain.UserAggregate.ValueObjects.Operations;
using LinqToDB.Mapping;

namespace Infrastructure.Common.Models;

[Table("operations")]
public sealed class Operation
{
    [Column("id")]
    public required Guid Id { get; set; }

    [Column("name")]
    public required string Name { get; set; }

    [Column("description")]
    public required string Description { get; set; }

    [Column("portfolio_id")]
    public Guid PortfolioId { get; set; }

    [Association(ThisKey = nameof(PortfolioId), OtherKey = nameof(Portfolio.Id))]
    public Portfolio Portfolio { get; set; }

    [Column("type")]
    public required OperationType Type { get; set; }

    [Column("state")]
    public required OperationState State { get; set; }

    [Column("instrument_id")]
    public required Guid? InstrumentId { get; set; }

    [Column("instrument_type")]
    public required InstrumentType InstrumentType { get; set; }

    [Column("quantity")]
    public required int Quantity { get; set; }

    [Column("rest_quantity")]
    public required int RestQuantity { get; set; }

    [Column("date")]
    public required DateTime Date { get; set; }

    [Column("payout")]
    public required decimal Payout { get; set; }

    [Column("price")]
    public required decimal Price { get; set; }

    [Column("commission")]
    public required decimal Commission { get; set; }

    [Association(ThisKey = nameof(Id), OtherKey = nameof(Trade.OperationId))]
    public List<Trade> Trades { get; set; }
}