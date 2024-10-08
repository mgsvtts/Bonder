using Bonder.Portfolio.Grpc;
using Domain.Common.Abstractions;
using Domain.UserAggregate.Repositories;
using Infrastructure.Common;
using Infrastructure.Token;
using Infrastructure.Users;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using Unchase.Swashbuckle.AspNetCore.Extensions.Extensions;

namespace Web;

public static class ProgramExtensions
{
    public static WebApplicationBuilder AddPresentation(this WebApplicationBuilder builder)
    {
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(options =>
        {
            options.AddEnumsWithValuesFixFilters();
        });

        builder.Services.AddControllers();

        return builder;
    }

    public static WebApplicationBuilder AddApplication(this WebApplicationBuilder builder)
    {
        builder.Services.AddSingleton<IJWTTokenGenerator, JwtTokenGenerator>(x =>
        {
            var section = builder.Configuration.GetSection("JWT");

            return new JwtTokenGenerator(section.GetValue<string>("Audience"),
                                         section.GetValue<string>("Issuer"),
                                         section.GetValue<string>("Key"),
                                         section.GetValue<int>("Lifetime"));
        });

        var routes = JsonNode.Parse(File.ReadAllText("access.json"))["Access"]
            .Deserialize<Dictionary<string, Application.Commands.CheckAccess.Dto.Route>>();

        builder.Services.AddSingleton(routes);

        builder.Services.AddMediator(config =>
        {
            config.ServiceLifetime = ServiceLifetime.Scoped;
        });

        Log.Logger = new LoggerConfiguration()
           .WriteTo.Console(outputTemplate: "[{Level}] {Timestamp:HH:mm:ss:ff} {Message}{NewLine}{Exception}")
           .CreateLogger();

        builder.Services.RegisterMapsterConfiguration();

        return builder;
    }

    public static WebApplicationBuilder AddInfrastructure(this WebApplicationBuilder builder)
    {
        builder.Services.AddDbContext<DatabaseContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("Database")));
        builder.Services.AddIdentity<Infrastructure.Common.Models.User, IdentityRole>()
                        .AddEntityFrameworkStores<DatabaseContext>()
                        .AddDefaultTokenProviders();

        builder.Services.AddGrpc();

        var portfolioServerUrl = new Uri(builder.Configuration.GetValue<string>("PortfolioServerUrl"));
        builder.Services.AddGrpcClient<PortfolioService.PortfolioServiceClient>(options => options.Address = portfolioServerUrl);

        builder.Services.AddAuthentication(x =>
        {
            x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(o =>
        {
            var key = Encoding.UTF8.GetBytes(builder.Configuration["JWT:Key"]);
            o.SaveToken = true;
            o.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = builder.Configuration["JWT:Issuer"],
                ValidAudience = builder.Configuration["JWT:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ClockSkew = TimeSpan.Zero
            };
            o.Events = new JwtBearerEvents
            {
                OnAuthenticationFailed = context =>
                {
                    if (context.Exception is SecurityTokenExpiredException)
                    {
                        context.Response.Headers.Append("TOKEN-EXPIRED", "true");
                    }
                    return Task.CompletedTask;
                }
            };
        });

        return builder;
    }

    public static WebApplicationBuilder AddDomain(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<IUserRepository, UserRepository>();

        return builder;
    }

    public static WebApplication AddMiddlewares(this WebApplication app)
    {
        if (!app.Environment.IsProduction())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthentication();

        app.UseAuthorization();

        app.MapControllers();

        return app;
    }
}