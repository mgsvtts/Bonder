using Domain.Common.Abstractions;
using Domain.UserAggregate.Entities;
using Domain.UserAggregate.ValueObjects.Operations;
using Domain.UserAggregate.ValueObjects.Portfolios;
using Domain.UserAggregate.ValueObjects.Users;
using System.Collections.Concurrent;

namespace Infrastructure;

public sealed class UserBuilder : IUserBuilder
{
    private readonly ITinkoffHttpClient _httpClient;

    public UserBuilder(ITinkoffHttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<Domain.UserAggregate.User> BuildAsync(UserId id, TinkoffToken tinkoffToken, CancellationToken token = default)
    {
        var portfolios = await _httpClient.GetPortfoliosAsync(tinkoffToken, token);

        var operations = await WaitAllAsync(tinkoffToken, portfolios, token);

        foreach (var portfolio in portfolios)
        {
            portfolio.AddOperations(operations[portfolio.Identity]);
        }

        return new Domain.UserAggregate.User(id, tinkoffToken, portfolios);
    }

    private async Task<IDictionary<PortfolioId, IEnumerable<Operation>>> WaitAllAsync(TinkoffToken tinkoffToken, IEnumerable<Portfolio> portfolios, CancellationToken token)
    {
        var dict = new ConcurrentDictionary<PortfolioId, IEnumerable<Operation>>();

        var waitList = portfolios
            .Select(async x =>
            {
                var operations = await _httpClient.GetOperationsAsync(tinkoffToken, x.AccountId.Value, token);

                dict.TryAdd(x.Identity, operations);
            })
            .ToList();

        await Task.WhenAll(waitList);

        return dict;
    }
}