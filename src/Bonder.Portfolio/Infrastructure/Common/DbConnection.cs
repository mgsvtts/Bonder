using Infrastructure.Common.Models;
using LinqToDB;
using LinqToDB.Data;

namespace Infrastructure.Common;

public sealed class DbConnection : DataConnection
{
    public ITable<User> Users => this.GetTable<User>();

    public DbConnection(DataOptions<DbConnection> options) : base(options.Options)
    {
    }
}