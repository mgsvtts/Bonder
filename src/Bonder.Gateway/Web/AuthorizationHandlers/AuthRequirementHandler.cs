
using Microsoft.AspNetCore.Authorization;
using System.Text;
using System.Text.Json;

namespace Web.AuthorizationHandlers;

public sealed class AuthRequirementHandler : AuthorizationHandler<AuthRequirement>
{
    private readonly HttpClient _client;
    private readonly string _authServerUrl;

    public AuthRequirementHandler(HttpClient httpClient, string authServerUrl)
    {
        _client = httpClient;
        _authServerUrl = authServerUrl;
    }

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, AuthRequirement requirement)
    {
        var isSuccess = await SetHeadersAsync(context.Resource as HttpContext);

        if (isSuccess)
        {
            context.Succeed(requirement);
        }
        else
        {
            context.Fail();
        }
    }

    private async Task<bool> SetHeadersAsync(HttpContext context)
    {
        var path = context.Request.Path != null ? context.Request.Path.Value : "";
        var content = new HttpRequestMessage
        {
            Content = new StringContent(JsonSerializer.Serialize(new { Path = path }), Encoding.UTF8, "application/json"),
            RequestUri = new Uri(_authServerUrl + "/api/auth/check-access"),
            Method = HttpMethod.Post
        };

        var accessToken = context.Request.Headers.FirstOrDefault(x => x.Key.Equals("X-ACCESS-TOKEN", StringComparison.OrdinalIgnoreCase));

        if(accessToken.Key is null || string.IsNullOrEmpty(accessToken.Value))
        {
            throw new InvalidOperationException("Provide an access token");
        }

        content.Headers.Add(accessToken.Key, accessToken.Value.ToString());

        var response = await _client.SendAsync(content, context.RequestAborted);

        foreach (var header in response.Headers.Where(x => x.Key.StartsWith("X-")))
        {
            context.Response.Headers.Append(header.Key, header.Value.First().ToString());
        }

        return response.IsSuccessStatusCode;
    }
}
