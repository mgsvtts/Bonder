using Application.Common.Abstractions;
using Domain.UserAggregate.Exceptions;
using Domain.UserAggregate.ValueObjects.Portfolios;
using Infrastructure.Dto.GetAccounts;
using Infrastructure.Dto.GetPortfolios;
using Mapster;
using MapsterMapper;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace Infrastructure;

public sealed class TinkoffHttpClient : ITinkoffHttpClient
{
    private readonly HttpClient _client;
    private readonly string _tinkoffUserServiceUrl;
    private readonly string _tinkoffOperationsServiceUrl;

    public TinkoffHttpClient(HttpClient client,
                             string tinkoffUrl,
                             string tinkoffOperationsServiceUrl)
    {
        _client = client;
        _tinkoffUserServiceUrl = tinkoffUrl;
        _tinkoffOperationsServiceUrl = tinkoffOperationsServiceUrl;
    }

    public async Task<IEnumerable<Portfolio>> GetPortfoliosAsync(string token, CancellationToken cancellationToken = default)
    {
        var content = new HttpRequestMessage
        {
            Content = new StringContent("{}", Encoding.UTF8, "application/json"),
            RequestUri = new Uri(_tinkoffUserServiceUrl + "/GetAccounts"),
            Method = HttpMethod.Post
        };

        content.Headers.Add("Authorization", $"Bearer {token}");

        var response = await _client.SendAsync(content, cancellationToken);

        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            throw new InvalidTokenException();
        }

        response.EnsureSuccessStatusCode();

        var serializedResponse = await response.Content.ReadFromJsonAsync<GetAccountsResponse>(cancellationToken: cancellationToken);

        var tasks = serializedResponse.Accounts
        .Where(x => x.Type == TinkoffAccountType.IIS || x.Type == TinkoffAccountType.Ordinary)
        .Select(x => GetPortfolioAsync(new GetPortfoliosRequest(x, token)))
        .ToList();

        await Task.WhenAll(tasks);

        return tasks.Select(x => x.Result);
    }

    private async Task<Portfolio> GetPortfolioAsync(GetPortfoliosRequest request, CancellationToken cancellationToken = default)
    {
        var content = new HttpRequestMessage
        {
            Content = new StringContent(JsonSerializer.Serialize(new { account_id = request.Account.Id, currency = "RUB" }), Encoding.UTF8, "application/json"),
            RequestUri = new Uri(_tinkoffOperationsServiceUrl + "/GetPortfolio"),
            Method = HttpMethod.Post
        };

        content.Headers.Add("Authorization", $"Bearer {request.Token}");

        var response = await _client.SendAsync(content, cancellationToken);

        response.EnsureSuccessStatusCode();
        var a = await response.Content.ReadAsStringAsync();
        var serializedResponse = await response.Content.ReadFromJsonAsync<GetTinkoffPortfolioResponse>(cancellationToken: cancellationToken);
        serializedResponse.Positions = serializedResponse.Positions.Where(x => x.Type == "bond");

        return (serializedResponse, request.Account).Adapt<Portfolio>();
    }
}