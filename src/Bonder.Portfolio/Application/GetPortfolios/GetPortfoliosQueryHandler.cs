using Application.Common.Abstractions;
using Bonder.Auth;
using Domain.UserAggregate.Abstractions.Repositories;
using Domain.UserAggregate.ValueObjects;
using Domain.UserAggregate.ValueObjects.Portfolios;
using Grpc.Core;
using MediatR;

namespace Application.GetPortfolios;

public sealed class GetPortfoliosQueryHandler : IRequestHandler<GetPortfoliosQuery, IEnumerable<Portfolio>>
{
    private readonly IUserBuilder _userBuilder;
    private readonly IUserRepository _portfolioRepository;
    private readonly UserService.UserServiceClient _grpcClient;

    public GetPortfoliosQueryHandler(IUserRepository portfolioRepository, IUserBuilder userBuilder, UserService.UserServiceClient grpcClient)
    {
        _portfolioRepository = portfolioRepository;
        _userBuilder = userBuilder;
        _grpcClient = grpcClient;
    }

    public async Task<IEnumerable<Portfolio>> Handle(GetPortfoliosQuery request, CancellationToken cancellationToken)
    {
        UserName userName;
        try
        {
            var response = await _grpcClient.GetUserByTokenAsync(new GetUserByTokenRequest
            {
                Token = request?.IdentityToken?.Split(" ").Last()
            }, cancellationToken: cancellationToken);

            userName = new UserName(response.UserName);
        }
        catch (RpcException ex) when (ex.StatusCode == StatusCode.Unauthenticated)
        {
            throw new ArgumentException("You does not have access to this call");
        }

        var user = await _portfolioRepository.GetByUserNameAsync(userName, cancellationToken);

        return user.Portfolios;
    }
}