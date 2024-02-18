using Application.Claims.Add;
using Application.Claims.Remove;
using Application.Login;
using Domain.UserAggregate;
using Domain.UserAggregate.ValueObjects;
using Mapster;
using MapsterMapper;
using Presentation.Controllers.Dto.AddClaims;
using Presentation.Controllers.Dto.RemoveClaims;
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
            Id = x.Identity.Identity.ToString(),
            UserName = x.UserName.Name,
            RefreshToken = x.Tokens.RefreshToken
        });

        TypeAdapterConfig<(string CurrentUser, AddClaimRequest Request), AddClaimsCommand>
        .ForType()
        .MapWith(x => new AddClaimsCommand(new UserName(x.CurrentUser), new UserName(x.Request.UserName), x.Request.Claims.Adapt<IEnumerable<Claim>>()));

        TypeAdapterConfig<(string CurrentUser, RemoveClaimRequest Request), RemoveClaimsCommand>
        .ForType()
        .MapWith(x => new RemoveClaimsCommand(new UserName(x.CurrentUser), new UserName(x.Request.UserName), x.Request.Claims));

        TypeAdapterConfig<UserClaim, Claim>
        .ForType()
        .MapWith(x => new Claim(x.ClaimName, x.ClaimValue));

        TypeAdapterConfig<User, AddClaimResponse>
        .ForType()
        .MapWith(x => new AddClaimResponse(x.UserName.Name, x.Claims.Adapt<IEnumerable<UserClaim>>()));

        TypeAdapterConfig<Claim, UserClaim>
        .ForType()
        .MapWith(x => new UserClaim(x.Type, x.Value));

        TypeAdapterConfig<Presentation.Controllers.Dto.Register.LoginRequest, LoginCommand>
        .ForType()
        .MapWith(x => new LoginCommand(new UserName(x.UserName), x.Password));

        TypeAdapterConfig<(Infrastructure.Common.Models.User User, IEnumerable<Claim> Claims), User>
                        .ForType()
                        .MapWith(x => new User(new UserId(Guid.Parse(x.User.Id)),
                                               new UserName(x.User.UserName),
                                               x.Claims,
                                               new Tokens(x.User.RefreshToken, null)));

        TypeAdapterConfig<(Infrastructure.Common.Models.User User, IList<Claim> Claims), User>
                .ForType()
                .MapWith(x => new User(new UserId(Guid.Parse(x.User.Id)),
                                       new UserName(x.User.UserName),
                                       x.Claims,
                                       new Tokens(x.User.RefreshToken, null)));

        TypeAdapterConfig.GlobalSettings.Scan(Assembly.GetExecutingAssembly());

        var typeAdapterConfig = TypeAdapterConfig.GlobalSettings;
        typeAdapterConfig.Scan(Assembly.GetExecutingAssembly());
        var mapperConfig = new Mapper(typeAdapterConfig);

        services.AddSingleton<IMapper>(mapperConfig);
    }
}