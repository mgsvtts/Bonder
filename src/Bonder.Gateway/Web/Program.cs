
using Microsoft.AspNetCore.Authorization;
using Web.AuthorizationHandlers;

namespace Web;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddReverseProxy()
            .LoadFromConfig(builder.Configuration.GetSection("Yarp"));

        builder.Services.AddHttpClient();
        builder.Services.AddScoped<IAuthorizationHandler, AuthRequirementHandler>(x =>
        {
            return new AuthRequirementHandler(x.GetRequiredService<HttpClient>(),
                                              builder.Configuration["Yarp:Clusters:auth-cluster:Destinations:auth-destination:Address"]);
        });

        builder.Services.AddAuthentication().AddBearerToken();

        builder.Services.AddAuthorization(x =>
        {
            x.AddPolicy("check-access", policy =>
            {
                policy.Requirements.Add(new AuthRequirement());
            });

            x.DefaultPolicy = x.GetPolicy("check-access");
        });


        var app = builder.Build();

        app.UseAuthentication();

        app.UseAuthorization();

        app.MapReverseProxy();

        await app.RunAsync();
    }
}