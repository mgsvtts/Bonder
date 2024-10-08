﻿using Application.Commands.ImportPortfolio.Common;
using Bonder.Calculation.Grpc;
using Domain.Common.Abstractions;
using Domain.UserAggregate.Abstractions.Repositories;
using Infrastructure.Common;
using Infrastructure.HttpClients;
using Infrastructure.Repositories;
using LinqToDB;
using LinqToDB.AspNet;
using LinqToDB.AspNet.Logging;
using NPOI.SS.Formula.Functions;
using Presentation.Grpc;
using RateLimiter;
using Serilog;
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

        DbConnection.Bind(builder.Configuration.GetConnectionString("Database"));

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

        Serilog.Log.Logger = new LoggerConfiguration()
               .WriteTo.Console(outputTemplate: "[{Level}] {Timestamp:HH:mm:ss:ff} {Message}{NewLine}{Exception}")
               .CreateLogger();

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
        builder.Services.AddScoped<IUserRepository, UserRepository>();

        var rateLimiter = TimeLimiter.GetFromMaxCountByInterval(1, TimeSpan.FromMilliseconds(201));
        builder.Services.AddSingleton(rateLimiter);

        builder.Services.AddHttpClient<ITinkoffHttpClient, TinkoffHttpClient>((httpClient, services) =>
        {
            return new TinkoffHttpClient(httpClient,
                                         builder.Configuration.GetValue<string>("TinkoffUsersServerUrl"),
                                         builder.Configuration.GetValue<string>("TinkoffOperatoinsServerUrl"));
        }).AddHttpMessageHandler(rateLimiter.AsDelegate);

        builder.Services.AddScoped<IPortfolioImporter, PortfolioImporter>();

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

        app.UseOutputCache();

        app.UseAuthorization();

        app.MapGrpcService<PortfolioServiceImpl>();

        app.MapControllers();

        return app;
    }
}