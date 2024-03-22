
using Microsoft.AspNetCore.Authorization;

namespace Web;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddReverseProxy()
            .LoadFromConfig(builder.Configuration.GetSection("Yarp"));

        builder.Services.AddHttpClient();
        builder.Services.AddTransient<IAuthorizationHandler, AuthRequirementHandler>();

        builder.Services.AddAuthorizationBuilder()
        .AddPolicy("check-access", policy =>
        {
            policy.Requirements.Add(new AuthRequirement());
        });

        var app = builder.Build();

        app.MapReverseProxy();

        app.Run();
    }
}
public sealed class AuthRequirement : IAuthorizationRequirement 
{ 

}

public sealed class AuthRequirementHandler : AuthorizationHandler<AuthRequirement>
{
    private readonly HttpClient _httpClient;

    public AuthRequirementHandler(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, AuthRequirement requirement)
    {
        var httpContext = context.Resource as HttpContext;



        context.Succeed(requirement);
    }
}
