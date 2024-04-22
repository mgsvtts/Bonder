using Microsoft.Extensions.Options;

namespace Web.Extensions;

public static class WebHookExtensions
{
    public static T GetConfiguration<T>(this IServiceProvider serviceProvider)
        where T : class
    {
        var config = serviceProvider.GetService<IOptions<T>>()
            ?? throw new ArgumentNullException(nameof(T));

        return config.Value;
    }

    public static ControllerActionEndpointConventionBuilder MapBotWebhookRoute<T>(this IEndpointRouteBuilder endpoints, string route)
    {
        var controllerName = typeof(T).Name.Replace("Controller", "", StringComparison.Ordinal);
        var actionName = typeof(T).GetMethods()[0].Name;

        return endpoints.MapControllerRoute(
            name: "bot_webhook",
            pattern: route,
            defaults: new { controller = controllerName, action = actionName });
    }
}