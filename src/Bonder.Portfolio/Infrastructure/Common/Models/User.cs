using LinqToDB.Mapping;

namespace Infrastructure.Common.Models;

[Table("users")]
public sealed class User
{
    [PrimaryKey]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("token")]
    public string Token { get; set; }

    [Association(ThisKey = nameof(Id), OtherKey = nameof(Portfolio.UserId))]
    public List<Portfolio> Portfolios { get; set; }
}