using Application.Commands.Claims.Add;
using Application.Commands.Claims.Remove;
using Application.Commands.Login;
using Domain.UserAggregate;
using Domain.UserAggregate.ValueObjects;
using Mapster;
using MapsterMapper;
using Presentation.Controllers.Dto.AddClaims;
using Presentation.Controllers.Dto.Login;
using Presentation.Controllers.Dto.RemoveClaims;
using Shared.Domain.Common.ValueObjects;
using System.Reflection;
using System.Security.Claims;

namespace Web;

public static class MapsterConfig
{
    public static void RegisterMapsterConfiguration(this IServiceCollection services)
    {
        TypeAdapterConfig<User, Infrastructure.Common.Models.User>
        .ForType()
        .MapWith(x => new Infrastructure.Common.Models.User
        {
            Id = x.Identity.Value.ToString(),
            UserName = x.UserName.ToString(),
            RefreshToken = x.Tokens.RefreshToken
        });

        TypeAdapterConfig<AddClaimRequest, AddClaimsCommand>
        .ForType()
        .MapWith(x => new AddClaimsCommand(new UserId(x.CurrentUserId), new ValidatedString(x.AddTo), x.Claims.Adapt<IEnumerable<Claim>>()));

        TypeAdapterConfig<(string CurrentUser, RemoveClaimRequest Request), RemoveClaimsCommand>
        .ForType()
        .MapWith(x => new RemoveClaimsCommand(new ValidatedString(x.CurrentUser), new ValidatedString(x.Request.UserName), x.Request.Claims));

        TypeAdapterConfig<UserClaim, Claim>
        .ForType()
        .MapWith(x => new Claim(x.ClaimName, x.ClaimValue));

        TypeAdapterConfig<User, AddClaimResponse>
        .ForType()
        .MapWith(x => new AddClaimResponse(x.UserName.ToString(), x.Claims.Adapt<IEnumerable<UserClaim>>()));

        TypeAdapterConfig<Claim, UserClaim>
        .ForType()
        .MapWith(x => new UserClaim(x.Type, x.Value));

        TypeAdapterConfig<LoginRequest, LoginCommand>
        .ForType()
        .MapWith(x => new LoginCommand(new ValidatedString(x.UserName), x.Password));

        TypeAdapterConfig<(Infrastructure.Common.Models.User User, IList<Claim> Claims), User>
                .ForType()
                .MapWith(x => new User(new UserId(Guid.Parse(x.User.Id)),
                                       new ValidatedString(x.User.UserName),
                                       x.Claims,
                                       new Tokens(x.User.RefreshToken, null)));

        TypeAdapterConfig.GlobalSettings.Scan(Assembly.GetExecutingAssembly());

        var typeAdapterConfig = TypeAdapterConfig.GlobalSettings;
        typeAdapterConfig.Scan(Assembly.GetExecutingAssembly());
        var mapperConfig = new Mapper(typeAdapterConfig);

        services.AddSingleton<IMapper>(mapperConfig);
    }
}