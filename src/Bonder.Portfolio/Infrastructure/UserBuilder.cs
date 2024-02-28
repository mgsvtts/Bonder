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

    public async Task<Domain.UserAggregate.User> BuildAsync(UserName userName, string tinkoffToken, CancellationToken cancellationToken = default)
    {
        var portfoliosTask = _httpClient.GetPortfoliosAsync(tinkoffToken, cancellationToken);

        var userTask = _grpcClient.GetUserByUserNameAsync(new GetUserByUserNameRequest
        {
            UserName = userName.Name
        }, cancellationToken: cancellationToken);

        await Task.WhenAll(portfoliosTask, userTask.ResponseAsync);

        if (string.IsNullOrEmpty(userTask.ResponseAsync.Result.Id))
        {
            throw new ArgumentException($"User {userName.Name} not exist");
        }

        return new Domain.UserAggregate.User(userName, tinkoffToken, portfoliosTask.Result);
    }
}