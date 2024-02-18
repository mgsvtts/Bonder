using LinqToDB.Mapping;

namespace Infrastructure.Common.Models;

[Table("users")]
public sealed class User
{
    [PrimaryKey]
    [Column("user_name")]
    public string UserName { get; set; }

    [Column("token")]
    public string Token { get; set; }
}