using Application.Common;
using Bonder.Auth;
using Domain.UserAggregate.Repositories;
using Domain.UserAggregate.ValueObjects;
using Grpc.Core;

namespace Infrastructure.GrpcServer;

public sealed class UserServiceImpl : UserService.UserServiceBase
{
    private readonly IUserRepository _userRepository;
    private readonly IJWTTokenGenerator _tokenGenerator;

    public UserServiceImpl(IUserRepository userRepository, IJWTTokenGenerator tokenGenerator)
    {
        _userRepository = userRepository;
        _tokenGenerator = tokenGenerator;
    }

    public override async Task<User> GetUserByUserName(GetUserByUserNameRequest request, ServerCallContext context)
    {
        var user = await _userRepository.GetByUserNameAsync(new UserName(request.UserName), context.CancellationToken);

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
            var principal = await _tokenGenerator.ValidateTokenAsync(request.Token, true);

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