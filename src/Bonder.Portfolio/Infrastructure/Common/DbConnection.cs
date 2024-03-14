using Infrastructure.Common.Models;
using LinqToDB;
using LinqToDB.Data;

namespace Infrastructure.Common;

public sealed class DbConnection : DataConnection
{
    public ITable<User> Users => this.GetTable<User>();
    public ITable<Portfolio> Portfolios => this.GetTable<Portfolio>();
    public ITable<Operation> Operations => this.GetTable<Operation>();
    public ITable<PortfolioBonds> PortfolioBonds => this.GetTable<PortfolioBonds>();

    public DbConnection(DataOptions<DbConnection> options) : base(options.Options)
    {
    }
}