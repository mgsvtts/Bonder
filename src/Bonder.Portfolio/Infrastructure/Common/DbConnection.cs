using Infrastructure.Common.Models;
using LinqToDB;
using LinqToDB.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Common;
public sealed class DbConnection : DataConnection
{
    public ITable<User> Users => this.GetTable<User>();

    public DbConnection(DataOptions<DbConnection> options) : base(options.Options)
    {
    }
}