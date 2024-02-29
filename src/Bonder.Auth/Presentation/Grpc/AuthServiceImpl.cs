using Application.Queries.GetPrincipalFromToken;
using Application.Queries.GetUserByUserName;
using Bonder.Auth.Grpc;
using Domain.UserAggregate.ValueObjects;
using Grpc.Core;
using MediatR;

namespace Presentation.Grpc;

public sealed class AuthServiceImpl : AuthService.AuthServiceBase
{
    private readonly ISender _sender;

    public AuthServiceImpl(ISender sender)
    {
        _sender = sender;
    }

    public override async Task<User> GetUserByUserName(GetUserByUserNameRequest request, ServerCallContext context)
    {
        var user = await _sender.Send(new GetUserByUserNameQuery(new UserName(request.UserName)), context.CancellationToken);

        return new User
        {
            Id = user?.Identity.ToString() ?? "",
            UserName = request.UserName,
            IsAdmin = user?.IsAdmin ?? false
        };
    }

    public override async Task<GetUserByTokenResponse> GetUserByToken(GetUserByTokenRequest request, ServerCallContext context)
    {
        try
        {
            var principal = await _sender.Send(new GetPrincipalFromTokenQuery(request.Token), context.CancellationToken);

            return new GetUserByTokenResponse
            {
                UserName = principal?.Identity?.Name ?? ""
            };
        }
        catch (Exception ex)
        {
            throw new RpcException(new Status(StatusCode.Unauthenticated, ex.Message, ex));
        }
    }
}