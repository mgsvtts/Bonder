using Infrastructure.Common.Models;
using LinqToDB;
using LinqToDB.Data;
using LinqToDB.DataProvider;
using LinqToDB.DataProvider.PostgreSQL;

namespace Infrastructure.Common;

public sealed class DbConnection : DataConnection
{
    public ITable<Bond> Bonds => this.GetTable<Bond>();
    public ITable<Coupon> Coupons => this.GetTable<Coupon>();
    public ITable<Amortization> Amortizations => this.GetTable<Amortization>();

    public DbConnection(DataOptions<DbConnection> options) : base(options.Options)
    { }

    public DbConnection() : base("Default")
    { }

    public static void Bind(string connection)
    {
        AddConfiguration("Default", connection, PostgreSQLTools.GetDataProvider(connectionString: connection));
    }
}