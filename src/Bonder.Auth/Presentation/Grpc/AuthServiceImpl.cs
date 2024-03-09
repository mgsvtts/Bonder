using Application.Queries.GetPrincipalFromToken;
using Application.Queries.GetUserById;
using Bonder.Auth.Grpc;
using Domain.UserAggregate.ValueObjects;
using Grpc.Core;
using Mediator;

namespace Presentation.Grpc;

public sealed class AuthServiceImpl : AuthService.AuthServiceBase
{
    private readonly ISender _sender;

    public AuthServiceImpl(ISender sender)
    {
        _sender = sender;
    }

    public override async Task<GrpcUser> GetUserById(GetUserByUserNameRequest request, ServerCallContext context)
    {
        var user = await _sender.Send(new GetUserByIdQuery(new UserId(Guid.Parse(request.UserId))), context.CancellationToken);

        return new GrpcUser
        {
            Id = user?.Identity.ToString() ?? "",
            UserName = user?.UserName is not null ? user?.UserName.ToString() : "",
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
                UserId = principal?.Identity?.Name ?? ""
            };
        }
        catch (Exception ex)
        {
            throw new RpcException(new Status(StatusCode.Unauthenticated, ex.Message, ex));
        }
    }
}