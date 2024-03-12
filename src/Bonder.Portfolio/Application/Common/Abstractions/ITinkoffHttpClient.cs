using Domain.UserAggregate.Entities;
using Domain.UserAggregate.ValueObjects.Operations;
using Domain.UserAggregate.ValueObjects.Portfolios;
using Domain.UserAggregate.ValueObjects.Users;

namespace Application.Common.Abstractions;

public interface ITinkoffHttpClient
{
    Task<List<Operation>> GetOperationsAsync(TinkoffToken tinkoffToken, AccountId accountId, CancellationToken token);

    Task<IEnumerable<Portfolio>> GetPortfoliosAsync(TinkoffToken tinkoffToken, CancellationToken cancellationToken = default);
}