using LinqToDB.Mapping;

namespace Infrastructure.Common.Models;

[Table("users")]
public sealed class User
{
    [PrimaryKey]
    [Column("user_name")]
    public required string UserName { get; set; }

    [Column("token")]
    public required string Token { get; set; }

    [Association(ThisKey = nameof(UserName), OtherKey = nameof(Portfolio.UserName))]
    public required List<Portfolio> Portfolios { get; set; }
}
