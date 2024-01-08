using Application.Calculation.CalculateAll;
using Application.Calculation.Common.CalculationService;
using Application.Calculation.Common.Interfaces;
using Application.Common;
using Domain.BondAggreagte.Repositories;
using Infrastructure.Calculation.CalculateAll;
using Infrastructure.Calculation.Common;
using Infrastructure.Common;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using Presentation.Middlewares;
using RateLimiter;
using Web.Extensions;
using Web.Extensions.Mapping;

namespace Web;

public static class ProgramExtensions
{
    public static WebApplicationBuilder AddInfrastructure(this WebApplicationBuilder builder)
    {
        builder.Services.AddInvestApiClient((_, settings) => settings.AccessToken = builder.Configuration.GetValue<string>("TinkoffToken"));

        builder.Services.AddTransient<ITinkoffGrpcClient, TinkoffGrpcClient>();

        builder.Services.AddTransient<IAllBondsReceiver, AllBondsReceiver>();

        var rateLimiter = TimeLimiter.GetFromMaxCountByInterval(1, TimeSpan.FromMilliseconds(201));

        builder.Services.AddHttpClient<ITInkoffHttpClient, TinkoffHttpClient>((httpClient, services) =>
        {
            return new TinkoffHttpClient(httpClient,
                                         services.GetRequiredService<ITinkoffGrpcClient>(),
                                         services.GetRequiredService<IMapper>(),
                                         services.GetRequiredService<IDohodHttpClient>(),
                                         builder.Configuration.GetValue<string>("TinkoffToken"),
                                         builder.Configuration.GetValue<string>("TinkoffServerUrl"));
        }).AddHttpMessageHandler(rateLimiter.AsDelegate);

        builder.Services.AddHttpClient<IDohodHttpClient, DohodHttpClient>(httpClient =>
        {
            return new DohodHttpClient(httpClient, builder.Configuration.GetValue<string>("DohodServerUrl"));
        }).AddHttpMessageHandler(rateLimiter.AsDelegate);

        builder.Services.AddSingleton(rateLimiter);

        var databasePath = builder.Configuration.GetValue<string>("ConnectionString").Replace("???", builder.Configuration.GetValue<string>("DatabaseName"));
        builder.Services.AddDbContext<DataContext>((options) => options.UseSqlite(databasePath));

        builder.Services.AddTransient<IBondRepository, BondRepository>();

        return builder;
    }

    public static WebApplicationBuilder AddApplication(this WebApplicationBuilder builder)
    {
        builder.Services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssemblies(typeof(AssemblyReference).Assembly);
        });

        builder.Services.AddHostedService<BackgroundBondUpdater>();

        builder.Services.RegisterMapsterConfiguration();

        builder.Services.AddSingleton<ICalculationService, CalculationService>();

        return builder;
    }

    public static WebApplicationBuilder AddPresentation(this WebApplicationBuilder builder)
    {
        builder.Services.AddControllers(options =>
        {
            options.Filters.Add<CustomExceptionFilter>();
        });

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

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