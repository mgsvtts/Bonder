using Bonder.Calculation.Grpc;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Web.Controllers;
using Web.Extensions;
using Web.Services;
using Web.Services.Hosted;

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

        builder.Services.AddScoped<Bot>();

        builder.Services.AddHostedService<ConfigureWebhook>();

        builder.Services.AddControllers()
                        .AddNewtonsoftJson();

        builder.Services.AddGrpc();

        var portfolioServerUrl = new Uri(builder.Configuration.GetValue<string>("CalculationServerUrl"));
        builder.Services.AddGrpcClient<CalculationService.CalculationServiceClient>(options => options.Address = portfolioServerUrl);

        builder.Services.RegisterMapsterConfiguration();

        var app = builder.Build();

        app.MapBotWebhookRoute<BotController>(route: botConfiguration.Route);

        app.MapControllers();

        app.Run();
    }
}

public class BotConfiguration
{
    public static readonly string Configuration = "BotConfiguration";

    public string BotToken { get; init; } = default!;
    public string HostAddress { get; init; } = default!;
    public string Route { get; init; } = default!;
    public string SecretToken { get; init; } = default!;
}
