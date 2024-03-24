using Domain.UserAggregate.ValueObjects.Operations;
using Infrastructure.Common;
using LinqToDB;
using Mapster;
using Mediator;

namespace Application.Queries.GetOperations;

public sealed class GetOperationsQueryHandler : IRequestHandler<GetOperationsQuery, GetOperationsResponse>
{
    public async ValueTask<GetOperationsResponse> Handle(GetOperationsQuery request, CancellationToken token)
    {
        using var db = new DbConnection();

        var query = db.Operations
        .Where(x => x.PortfolioId == request.PortfolioId.Value);

        var operations = await query
        .OrderByDescending(x => x.Date)
        .Skip((request.PageInfo.CurrentPage - 1) * request.PageInfo.ItemsOnPage)
        .Take(request.PageInfo.ItemsOnPage)
        .LoadWith(x => x.Trades)
        .ToListAsync(token: token);

        return new GetOperationsResponse(operations.Adapt<IEnumerable<Operation>>(), request.PageInfo.Recreate(operations, await query.CountAsync(token)));
    }
}