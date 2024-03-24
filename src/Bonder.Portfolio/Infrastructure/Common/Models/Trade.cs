using LinqToDB.Mapping;

namespace Infrastructure.Common.Models;

[Table("trades")]
public sealed class Trade
{
    [Column("id")]
    public required Guid Id { get; set; }

    [Column("operation_id")]
    public Guid OperationId { get; set; }

    [Column("date")]
    public required DateTime Date { get; set; }

    [Column("quantity")]
    public required decimal Quantity { get; set; }

    [Column("price")]
    public required decimal Price { get; set; }
}