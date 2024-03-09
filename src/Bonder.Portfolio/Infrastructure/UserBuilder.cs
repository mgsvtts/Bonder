using Application.Common.Abstractions;
using Bonder.Auth.Grpc;
using Domain.UserAggregate.ValueObjects;

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

    public async Task<Domain.UserAggregate.User> BuildAsync(UserId id, string tinkoffToken, CancellationToken cancellationToken = default)
    {
        var portfoliosTask = _httpClient.GetPortfoliosAsync(tinkoffToken, cancellationToken);

        var userTask = _grpcClient.GetUserByIdAsync(new GetUserByUserNameRequest
        {
            UserId = id.Value.ToString()
        }, cancellationToken: cancellationToken);

        await Task.WhenAll(portfoliosTask, userTask.ResponseAsync);

        if (string.IsNullOrEmpty(userTask.ResponseAsync.Result.Id))
        {
            throw new ArgumentException($"User {id.Value} not exist");
        }

        return new Domain.UserAggregate.User(id, tinkoffToken, portfoliosTask.Result);
    }
}