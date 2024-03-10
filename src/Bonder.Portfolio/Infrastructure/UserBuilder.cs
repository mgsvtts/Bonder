using Application.Common.Abstractions;
using Bonder.Auth.Grpc;
using Domain.UserAggregate.Entities;
using Domain.UserAggregate.ValueObjects.Operations;
using Domain.UserAggregate.ValueObjects.Portfolios;
using Domain.UserAggregate.ValueObjects.Users;
using Grpc.Core;
using System.Collections.Concurrent;

namespace Infrastructure;

public sealed class UserBuilder : IUserBuilder
{
    private readonly ITinkoffHttpClient _httpClient;
    private readonly AuthService.AuthServiceClient _grpcClient;

    public UserBuilder(ITinkoffHttpClient httpClient, AuthService.AuthServiceClient grpcClient)
    {
        _httpClient = httpClient;
        _grpcClient = grpcClient;
    }

    public async Task<Domain.UserAggregate.User> BuildAsync(UserId id, TinkoffToken tinkoffToken, CancellationToken cancellationToken = default)
    {
        var portfolios = await _httpClient.GetPortfoliosAsync(tinkoffToken, cancellationToken);

        var userTask = _grpcClient.GetUserByIdAsync(new GetUserByUserNameRequest
        {
            UserId = id.Value.ToString()
        }, cancellationToken: cancellationToken);

        var operations = await WaitAllAsync(tinkoffToken, portfolios, userTask, cancellationToken);

        if (string.IsNullOrEmpty(userTask.ResponseAsync.Result.Id))
        {
            throw new ArgumentException($"User {id.Value} not exist");
        }

        foreach (var portfolio in portfolios)
        {
            portfolio.AddOperations(operations[portfolio.Identity]);
        }

        return new Domain.UserAggregate.User(id, tinkoffToken, portfolios);
    }

    private async Task<IDictionary<PortfolioId, IEnumerable<Operation>>> WaitAllAsync(TinkoffToken tinkoffToken, IEnumerable<Portfolio> portfolios, AsyncUnaryCall<GrpcUser> userTask, CancellationToken cancellationToken)
    {
        var waitList = new List<Task>();
        var dict = new ConcurrentDictionary<PortfolioId, IEnumerable<Operation>>();

        var operationsTask = portfolios
        .Select(async x =>
        {
            var operations = await _httpClient.GetOperationsAsync(tinkoffToken, x.AccountId.Value, cancellationToken);

            dict.TryAdd(x.Identity, operations);
        })
        .ToList();

        waitList.AddRange(operationsTask);
        waitList.Add(userTask.ResponseAsync);

        await Task.WhenAll(waitList);

        return dict;
    }
}