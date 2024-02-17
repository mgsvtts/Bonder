using Infrastructure.Common.Models;
using LinqToDB;
using LinqToDB.Data;

namespace Infrastructure.Common;

public sealed class DbConnection : DataConnection
{
    public ITable<Bond> Bonds => this.GetTable<Bond>();
    public ITable<Coupon> Coupons => this.GetTable<Coupon>();

    public DbConnection(DataOptions<DbConnection> options) : base(options.Options)
    {
    }
}