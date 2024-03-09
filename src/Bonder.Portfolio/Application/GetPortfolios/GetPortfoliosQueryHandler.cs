using Bonder.Auth.Grpc;
using Domain.UserAggregate.Abstractions.Repositories;
using Domain.UserAggregate.ValueObjects;
using Domain.UserAggregate.ValueObjects.Portfolios;
using Grpc.Core;
using Mediator;

namespace Application.GetPortfolios;

public sealed class GetPortfoliosQueryHandler : IQueryHandler<GetPortfoliosQuery, IEnumerable<Portfolio>>
{
    private readonly IUserRepository _portfolioRepository;
    private readonly AuthService.AuthServiceClient _grpcClient;

    public GetPortfoliosQueryHandler(IUserRepository portfolioRepository, AuthService.AuthServiceClient grpcClient)
    {
        _portfolioRepository = portfolioRepository;
        _grpcClient = grpcClient;
    }

    public async ValueTask<IEnumerable<Portfolio>> Handle(GetPortfoliosQuery request, CancellationToken cancellationToken)
    {
        UserId userName;
        try
        {
            var response = await _grpcClient.GetUserByTokenAsync(new GetUserByTokenRequest
            {
                Token = request?.IdentityToken?.Split(" ").Last()
            }, cancellationToken: cancellationToken);

            userName = new UserId(Guid.Parse(response.UserId));
        }
        catch (RpcException ex) when (ex.StatusCode == StatusCode.Unauthenticated)
        {
            throw new ArgumentException("You does not have access to this call");
        }

        var user = await _portfolioRepository.GetByIdAsync(userName, cancellationToken);

        return user.Portfolios;
    }
}