using Application.Calculation.CalculateAll.Services;
using Application.Calculation.Common.CalculationService;
using Application.Calculation.Common.Interfaces;
using Application.Common;
using Domain.BondAggreagte.Abstractions;
using Infrastructure.Calculation.CalculateAll;
using Infrastructure.Calculation.Common;
using Infrastructure.Common;
using LinqToDB;
using LinqToDB.AspNet;
using LinqToDB.AspNet.Logging;
using MapsterMapper;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.AspNetCore.RateLimiting;
using Presentation.Filters;
using RateLimiter;
using System.Threading.RateLimiting;
using Web.Extensions;
using Web.Extensions.Mapping;

namespace Web;

public static class ProgramExtensions
{
    public static WebApplicationBuilder AddInfrastructure(this WebApplicationBuilder builder)
    {
        builder.Services.AddInvestApiClient((_, settings) => settings.AccessToken = builder.Configuration.GetValue<string>("TinkoffToken"))
                        .AddRateLimiter(x => x.AddSlidingWindowLimiter("limiting", options =>
                        {
                            options.AutoReplenishment = true;
                            options.PermitLimit = 200;
                            options.Window = TimeSpan.FromMinutes(1);
                            options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                        }));

        builder.Services.AddTransient<ITinkoffGrpcClient, TinkoffGrpcClient>();

        builder.Services.AddTransient<IAllBondsReceiver, AllBondsReceiver>();

        var rateLimiter = TimeLimiter.GetFromMaxCountByInterval(1, TimeSpan.FromMilliseconds(201));

        builder.Services.AddHttpClient<ITInkoffHttpClient, TinkoffHttpClient>((httpClient, services) =>
        {
            return new TinkoffHttpClient(httpClient,
                                         services.GetRequiredService<IMapper>(),
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
                                      services.GetRequiredService<IMapper>(),
                                      builder.Configuration.GetValue<string>("MoexServerUrl"));
        }).AddHttpMessageHandler(rateLimiter.AsDelegate);

        builder.Services.AddSingleton(rateLimiter);

        builder.Services.AddLinqToDBContext<DbConnection>((provider, options)
         => options.UsePostgreSQL(builder.Configuration.GetConnectionString("Database"))
                   .UseDefaultLogging(provider));

        return builder;
    }

    public static WebApplicationBuilder AddApplication(this WebApplicationBuilder builder)
    {
        builder.Services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssemblies(typeof(AssemblyReference).Assembly);
        });

        if (builder.Configuration.GetValue<bool>("TurnOnDaemons"))
        {
            builder.Services.AddHostedService<BackgroundBondPriceUpdater>();
            builder.Services.AddHostedService<BackgroundBondUpdater>();
        }

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
        builder.Services.AddSwaggerGen();

        return builder;
    }

    public static WebApplicationBuilder AddDomain(this WebApplicationBuilder builder)
    {
        builder.Services.AddTransient<IBondRepository, BondRepository>();
        builder.Services.AddTransient<IBondBuilder, BondBuilder>();

        return builder;
    }

    public static WebApplication AddMiddlewares(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        app.UseWebSockets(new WebSocketOptions
        {
            KeepAliveInterval = TimeSpan.FromSeconds(15)
        });

        return app;
    }
}
