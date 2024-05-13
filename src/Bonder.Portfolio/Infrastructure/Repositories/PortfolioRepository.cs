using Domain.UserAggregate.Abstractions.Repositories;
using Domain.UserAggregate.ValueObjects.Operations;
using Domain.UserAggregate.ValueObjects.Portfolios;
using Infrastructure.Common;
using LinqToDB;
using Mapster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories;
public sealed class PortfolioRepository : IPortfolioRepository
{
    public async Task AddOperation(PortfolioId portfolioId, Operation operation, CancellationToken token)
    {
        var dbOperation = operation.Adapt<Operation>();

        using var db = new DbConnection();

        await db.InsertAsync(dbOperation, token: token);
    }
}
