using Application.Common.Abstractions;
using Bonder.Auth.Grpc;
using Domain.UserAggregate.Abstractions.Repositories;
using Domain.UserAggregate.ValueObjects;
using Domain.UserAggregate.ValueObjects.Portfolios;
using Grpc.Core;
using MediatR;

namespace Application.GetPortfolios;

public sealed class GetPortfoliosQueryHandler : IRequestHandler<GetPortfoliosQuery, IEnumerable<Portfolio>>
{
    private readonly IUserRepository _portfolioRepository;
    private readonly AuthService.AuthServiceClient _grpcClient;

    public GetPortfoliosQueryHandler(IUserRepository portfolioRepository, AuthService.AuthServiceClient grpcClient)
    {
        _portfolioRepository = portfolioRepository;
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