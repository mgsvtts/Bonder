using Application.Calculation.Common.CalculationService;
using Application.Calculation.Common.Interfaces;
using Application.Common;
using Infrastructure.Calculation;
using MapsterMapper;
using Presentation.Middlewares;
using Web.Extensions.Mapping;

namespace Web;

public static class ProgramExtensions
{
    public static WebApplicationBuilder AddInfrastructure(this WebApplicationBuilder builder)
    {
        builder.Services.AddInvestApiClient((_, settings) => settings.AccessToken = builder.Configuration.GetValue<string>("TinkoffToken"));

        builder.Services.AddTransient<ITinkoffGrpcClient, TinkoffGrpcClient>();

        builder.Services.AddHttpClient<ITInkoffHttpClient, TinkoffHttpClient>((httpClient, services) =>
        {
            return new TinkoffHttpClient(httpClient,
                                         services.GetRequiredService<ITinkoffGrpcClient>(),
                                         services.GetRequiredService<IMapper>(),
                                         services.GetRequiredService<IDohodHttpClient>(),
                                         builder.Configuration.GetValue<string>("TinkoffToken"),
                                         builder.Configuration.GetValue<string>("TinkoffServerUrl"));
        });

        builder.Services.AddHttpClient<IDohodHttpClient, DohodHttpClient>(httpClient =>
        {
            return new DohodHttpClient(httpClient, builder.Configuration.GetValue<string>("DohodServerUrl"));
        });

        return builder;
    }

    public static WebApplicationBuilder AddApplication(this WebApplicationBuilder builder)
    {
        builder.Services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssemblies(typeof(AssemblyReference).Assembly);
        });

        builder.Services.RegisterMapsterConfiguration();

        builder.Services.AddSingleton<ICalculator, Calculator>();

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

        return app;
    }
}