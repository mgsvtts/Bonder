using Application.Common;
using Application.Common.Abstractions;
using Bonder.Auth.Grpc;
using Bonder.Portfolio.Grpc;
using Domain.UserAggregate.Abstractions.Repositories;
using Infrastructure;
using Infrastructure.Common;
using LinqToDB;
using LinqToDB.AspNet;
using LinqToDB.AspNet.Logging;
using MapsterMapper;
using Presentation.Controllers;
using Presentation.Grpc;
using RateLimiter;
using Unchase.Swashbuckle.AspNetCore.Extensions.Extensions;
using Web.Extensions;

namespace Web;

public static class ProgramExtensions
{
    public static WebApplicationBuilder AddInfrastructure(this WebApplicationBuilder builder)
    {
        builder.Services.AddLinqToDBContext<DbConnection>((provider, options)
         => options.UsePostgreSQL(builder.Configuration.GetConnectionString("Database"))
                   .UseDefaultLogging(provider));

        builder.Services.AddGrpc();

        var authServerUrl = new Uri(builder.Configuration.GetValue<string>("AuthServerUrl"));
        builder.Services.AddGrpcClient<AuthService.AuthServiceClient>(options => options.Address = authServerUrl);

        var rateLimiter = TimeLimiter.GetFromMaxCountByInterval(1, TimeSpan.FromMilliseconds(201));

        builder.Services.AddHttpClient<ITinkoffHttpClient, TinkoffHttpClient>((httpClient, services) =>
        {
            return new TinkoffHttpClient(httpClient,
                                         builder.Configuration.GetValue<string>("TinkoffUsersServerUrl"),
                                         builder.Configuration.GetValue<string>("TinkoffOperatoinsServerUrl"));
        }).AddHttpMessageHandler(rateLimiter.AsDelegate);

        return builder;
    }

    public static WebApplicationBuilder AddApplication(this WebApplicationBuilder builder)
    {
        builder.Services.RegisterMapsterConfiguration();

        builder.Services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssemblies(typeof(AssemblyReference).Assembly);
        });

        builder.Services.AddTransient<IUserBuilder, UserBuilder>();

        return builder;
    }

    public static WebApplicationBuilder AddPresentation(this WebApplicationBuilder builder)
    {
        builder.Services.AddControllers();

        builder.Services.AddEndpointsApiExplorer();

        builder.Services.AddSwaggerGen(options =>
        {
            options.AddEnumsWithValuesFixFilters();
        });

        return builder;
    }

    public static WebApplicationBuilder AddDomain(this WebApplicationBuilder builder)
    {
        builder.Services.AddTransient<IUserRepository, UserRepository>();

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

        app.UseAuthorization();

        app.MapGrpcService<PortfolioServiceImpl>();

        app.MapControllers();

        return app;
    }
}