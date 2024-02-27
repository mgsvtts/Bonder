using Infrastructure.Common.Models;
using LinqToDB;
using LinqToDB.Data;

namespace Infrastructure.Common;

public sealed class DbConnection : DataConnection
{
    public ITable<User> Users => this.GetTable<User>();
    public ITable<PortfolioBonds>PortfolioBonds=> this.GetTable<PortfolioBonds>();
    public ITable<Portfolio> Portfolios=> this.GetTable<Portfolio>();

    public DbConnection(DataOptions<DbConnection> options) : base(options.Options)
    {
    }
}