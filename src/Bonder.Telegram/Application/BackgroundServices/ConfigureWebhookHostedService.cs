using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace Application.BackgroundServices;

public class ConfigureWebhook : IHostedService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly BotConfiguration _botConfig;

    public ConfigureWebhook(IServiceProvider serviceProvider, IOptions<BotConfiguration> botOptions)
    {
        _serviceProvider = serviceProvider;
        _botConfig = botOptions.Value;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await using var scope = _serviceProvider.CreateAsyncScope();
        var botClient = scope.ServiceProvider.GetRequiredService<ITelegramBotClient>();

        await botClient.DeleteWebhookAsync(cancellationToken: cancellationToken);

        await botClient.SetWebhookAsync(
             url: $"{_botConfig.HostAddress}{_botConfig.Route}",
             allowedUpdates: Array.Empty<UpdateType>(),
             secretToken: _botConfig.SecretToken,
             cancellationToken: cancellationToken);
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await using var scope = _serviceProvider.CreateAsyncScope();

        var botClient = scope.ServiceProvider.GetRequiredService<ITelegramBotClient>();

        await botClient.DeleteWebhookAsync(cancellationToken: cancellationToken);
    }
}