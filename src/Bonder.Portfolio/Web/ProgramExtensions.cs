using Bonder.Calculation.Grpc;
using Domain.Common.Abstractions;
using Domain.UserAggregate.Abstractions.Repositories;
using Infrastructure;
using Infrastructure.Common;
using LinqToDB;
using LinqToDB.AspNet;
using LinqToDB.AspNet.Logging;
using Presentation.Grpc;
using RateLimiter;
using Unchase.Swashbuckle.AspNetCore.Extensions.Extensions;
using Web.Extensions;

namespace Web;

public static class ProgramExtensions
{
    public static WebApplicationBuilder AddInfrastructure(this WebApplicationBuilder builder)
    {
        builder.Services.AddLinqToDBContext<DbConnection>((provider, options) =>
        {
            options = options.UsePostgreSQL(builder.Configuration.GetConnectionString("Database"));

            if (!builder.Environment.IsProduction())
            {
                options = options.UseDefaultLogging(provider);
            }

            return options;
        });

        builder.Services.AddGrpc();

        var calculationServerUrl = new Uri(builder.Configuration.GetValue<string>("CalculationServerUrl"));
        builder.Services.AddGrpcClient<CalculationService.CalculationServiceClient>(options => options.Address = calculationServerUrl);

        return builder;
    }

    public static WebApplicationBuilder AddApplication(this WebApplicationBuilder builder)
    {
        builder.Services.RegisterMapsterConfiguration();

        builder.Services.AddMediator(config =>
        {
            config.ServiceLifetime = ServiceLifetime.Scoped;
        });

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

        builder.Services.AddStackExchangeRedisOutputCache(x =>
        {
            x.Configuration = builder.Configuration.GetConnectionString("Redis");
        });

        return builder;
    }

    public static WebApplicationBuilder AddDomain(this WebApplicationBuilder builder)
    {
        builder.Services.AddTransient<IUserRepository, UserRepository>();

        var rateLimiter = TimeLimiter.GetFromMaxCountByInterval(1, TimeSpan.FromMilliseconds(201));
        builder.Services.AddSingleton(rateLimiter);

        builder.Services.AddHttpClient<ITinkoffHttpClient, TinkoffHttpClient>((httpClient, services) =>
        {
            return new TinkoffHttpClient(httpClient,
                                         builder.Configuration.GetValue<string>("TinkoffUsersServerUrl"),
                                         builder.Configuration.GetValue<string>("TinkoffOperatoinsServerUrl"));
        }).AddHttpMessageHandler(rateLimiter.AsDelegate);

        builder.Services.AddTransient<IUserBuilder, UserBuilder>();

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