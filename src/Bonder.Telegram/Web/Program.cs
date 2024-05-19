using Application.BackgroundServices;
using Application.Bot;
using Bonder.Calculation.Grpc;
using Bonder.Portfolio.Grpc;
using Presentation;
using Serilog;
using Telegram.Bot;
using Web.Extensions;

namespace Web;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        var botConfigurationSection = builder.Configuration.GetSection(BotConfiguration.Configuration);
        builder.Services.Configure<BotConfiguration>(botConfigurationSection);

        var botConfiguration = botConfigurationSection.Get<BotConfiguration>();

        builder.Services.AddHttpClient("telegram_bot_client")
        .AddTypedClient<ITelegramBotClient>((httpClient, sp) =>
        {
            var botConfig = sp.GetConfiguration<BotConfiguration>();
            var options = new TelegramBotClientOptions(botConfig.BotToken);
            return new TelegramBotClient(options, httpClient);
        });

        builder.Services.AddScoped<TelegramBot>();

        builder.Services.AddHostedService<ConfigureWebhook>();

        builder.Services.AddControllers()
                        .AddNewtonsoftJson();

        builder.Services.AddGrpc();

        var calculationServerUrl = new Uri(builder.Configuration.GetValue<string>("CalculationServerUrl"));
        var portfolioServerUrl = new Uri(builder.Configuration.GetValue<string>("PortfolioServerUrl"));
        builder.Services.AddGrpcClient<CalculationService.CalculationServiceClient>(options => options.Address = calculationServerUrl);
        builder.Services.AddGrpcClient<PortfolioService.PortfolioServiceClient>(options => options.Address = portfolioServerUrl);

        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console(outputTemplate: "[{Level}] {Timestamp:HH:mm:ss:ff} {Message}{NewLine}{Exception}")
            .CreateLogger();

        builder.Services.RegisterMapsterConfiguration();

        var app = builder.Build();

        app.MapBotWebhookRoute<BotController>(route: botConfiguration.Route);

        app.MapControllers();

        app.Run();
    }
}