using Application.Commands.Calculation.CalculateAll.Jobs;
using Application.Commands.Calculation.Common.CalculationService;
using Domain.BondAggreagte.Abstractions;
using Infrastructure.Calculation.CalculateAll;
using Infrastructure.Calculation.CalculateAll.Cache;
using Infrastructure.Calculation.CalculateAll.GrpcClients;
using Infrastructure.Calculation.CalculateAll.HttpClients;
using Infrastructure.Calculation.CalculateAll.Repositories;
using Infrastructure.Common;
using LinqToDB;
using LinqToDB.AspNet;
using LinqToDB.AspNet.Logging;
using Microsoft.AspNetCore.RateLimiting;
using Presentation.Filters;
using Presentation.Grpc;
using Quartz;
using RateLimiter;
using Serilog;
using System.Threading.RateLimiting;
using Unchase.Swashbuckle.AspNetCore.Extensions.Extensions;
using Web.Extensions;

namespace Web;

public static class ProgramExtensions
{
    public static WebApplicationBuilder AddInfrastructure(this WebApplicationBuilder builder)
    {
        builder.Services.AddGrpc();

        builder.Services.AddScoped<BondRepository>();
        builder.Services.AddScoped<CalculateAllService>();

        builder.Services.AddScoped<ICalculateAllService, CachedCalculateAllService>();
        builder.Services.AddScoped<IBondRepository, CachedBondRepository>();
        builder.Services.AddScoped<ITinkoffGrpcClient, TinkoffGrpcClient>();
        builder.Services.AddScoped<IAllBondsReceiver, AllBondsReceiver>();

        builder.Services.AddInvestApiClient((_, settings) => settings.AccessToken = builder.Configuration.GetValue<string>("TinkoffToken"))
                        .AddRateLimiter(x => x.AddSlidingWindowLimiter("limiting", options =>
                        {
                            options.AutoReplenishment = true;
                            options.PermitLimit = 200;
                            options.Window = TimeSpan.FromMinutes(1);
                            options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                        }));

        var rateLimiter = TimeLimiter.GetFromMaxCountByInterval(1, TimeSpan.FromMilliseconds(201));
        builder.Services.AddSingleton(rateLimiter);

        builder.Services.AddHttpClient<ITInkoffHttpClient, TinkoffHttpClient>((httpClient, services) =>
        {
            return new TinkoffHttpClient(httpClient,
                                         builder.Configuration.GetValue<string>("TinkoffToken"),
                                         builder.Configuration.GetValue<string>("TinkoffServerUrl"));
        }).AddHttpMessageHandler(rateLimiter.AsDelegate);

        builder.Services.AddHttpClient<IDohodHttpClient, DohodHttpClient>(httpClient =>
        {
            return new DohodHttpClient(httpClient,
                                       builder.Configuration.GetValue<string>("DohodServerUrl"));
        }).AddHttpMessageHandler(rateLimiter.AsDelegate);

        builder.Services.AddHttpClient<IMoexHttpClient, MoexHttpClient>((httpClient, services) =>
        {
            return new MoexHttpClient(httpClient,
                                      builder.Configuration.GetValue<string>("MoexServerUrl"));
        }).AddHttpMessageHandler(rateLimiter.AsDelegate);

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

        builder.Services.AddStackExchangeRedisCache(x =>
        {
            x.Configuration = builder.Configuration.GetConnectionString("Redis");
        });

        return builder;
    }

    public static WebApplicationBuilder AddApplication(this WebApplicationBuilder builder)
    {
        builder.Services.AddMediator(config =>
        {
            config.ServiceLifetime = ServiceLifetime.Scoped;
        });

        if (builder.Configuration.GetValue<bool>("TurnOnDaemons"))
        {
            builder.Services.AddQuartz(configure =>
            {
                var priceKey = new JobKey(nameof(UpdateBondPriceJob));
                var bondKey = new JobKey(nameof(BackgroundBondUpdater));

                configure.AddJob<UpdateBondPriceJob>(priceKey)
                .AddTrigger(trigger => trigger.ForJob(priceKey).WithSimpleSchedule(schedule => schedule.WithInterval(TimeSpan.FromMicroseconds(1))
                                                                                                       .RepeatForever()));
                configure.AddJob<BackgroundBondUpdater>(bondKey)
                .AddTrigger(trigger => trigger.ForJob(bondKey).WithCronSchedule("0 0 5 ? * * *"));
            });
            builder.Services.AddQuartzHostedService(options => options.WaitForJobsToComplete = true);
        }

        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console(outputTemplate: "[{Level}] {Timestamp:HH:mm:ss:ff} {Message}{NewLine}{Exception}")
            .CreateLogger();

        builder.Services.RegisterMapsterConfiguration();

        builder.Services.AddSingleton<ICalculationService, CalculationService>();

        return builder;
    }

    public static WebApplicationBuilder AddPresentation(this WebApplicationBuilder builder)
    {
        builder.Services.AddControllers(options =>
        {
            options.Filters.Add<ExceptionFilterAttribute>();
        });

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
        builder.Services.AddScoped<IBondBuilder, BondBuilder>();

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

        app.MapGrpcService<CalculationServiceImpl>();

        app.MapControllers();

        return app;
    }
}