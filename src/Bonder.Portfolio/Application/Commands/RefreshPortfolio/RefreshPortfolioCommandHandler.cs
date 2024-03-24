using Domain.Common.Abstractions;
using Domain.UserAggregate.Abstractions.Repositories;
using Domain.UserAggregate.Entities;
using Domain.UserAggregate.ValueObjects.Operations;
using Domain.UserAggregate.ValueObjects.Portfolios;
using Domain.UserAggregate.ValueObjects.Users;
using Mediator;
using System.Collections.Concurrent;

namespace Application.Commands.RefreshPortfolio;

public sealed class RefreshPortfolioCommandHandler : ICommandHandler<RefreshPortfolioCommand>
{
    private readonly IUserRepository _userRepository;
    private readonly ITinkoffHttpClient _httpClient;

    public RefreshPortfolioCommandHandler(IUserRepository portfolioRepository, ITinkoffHttpClient httpClient)
    {
        _userRepository = portfolioRepository;
        _httpClient = httpClient;
    }

    public async ValueTask<Unit> Handle(RefreshPortfolioCommand request, CancellationToken token)
    {
        var user = await GetUserAsync(request.UserId, request.Token, token);

        await _userRepository.SaveAsync(user, token);

        return Unit.Value;
    }

    public async Task<Domain.UserAggregate.User> GetUserAsync(UserId id, TinkoffToken tinkoffToken, CancellationToken token)
    {
        var portfolios = await _httpClient.GetPortfoliosAsync(tinkoffToken, token);

        var operations = await GetOperationsAsync(tinkoffToken, portfolios, token);

        foreach (var portfolio in portfolios)
        {
            portfolio.AddOperations(operations[portfolio.Identity]);
        }

        return new Domain.UserAggregate.User(id, tinkoffToken, portfolios);
    }

    private async Task<IDictionary<PortfolioId, IEnumerable<Operation>>> GetOperationsAsync(TinkoffToken tinkoffToken, IEnumerable<Portfolio> portfolios, CancellationToken token)
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