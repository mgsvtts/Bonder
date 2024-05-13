using Domain.Common.Abstractions;
using Domain.UserAggregate.Entities;
using Domain.UserAggregate.Exceptions;
using Domain.UserAggregate.ValueObjects.Operations;
using Domain.UserAggregate.ValueObjects.Portfolios;
using Domain.UserAggregate.ValueObjects.Users;
using Infrastructure.HttpClients.Dto.GetAccounts;
using Infrastructure.HttpClients.Dto.GetOperations;
using Infrastructure.HttpClients.Dto.GetPortfolios;
using Mapster;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace Infrastructure.HttpClients;

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

    public async Task<IEnumerable<Portfolio>> GetPortfoliosAsync(TinkoffToken tinkoffToken, CancellationToken token)
    {
        var content = new HttpRequestMessage
        {
            Content = new StringContent("{}", Encoding.UTF8, "application/json"),
            RequestUri = new Uri(_tinkoffUserServiceUrl + "/GetAccounts"),
            Method = HttpMethod.Post
        };

        content.Headers.Add("Authorization", $"Bearer {tinkoffToken}");

        var response = await _client.SendAsync(content, token);

        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            throw new InvalidTokenException();
        }

        response.EnsureSuccessStatusCode();

        var serializedResponse = await response.Content.ReadFromJsonAsync<GetTinkoffAccountsResponse>(cancellationToken: token);

        var tasks = serializedResponse.Accounts
        .Where(AccountIsValid)
        .Select(x => GetPortfolioAsync(x, tinkoffToken, token))
        .ToList();

        await Task.WhenAll(tasks);

        return tasks.Select(x => x.Result);
    }

    public async Task<List<Operation>> GetOperationsAsync(TinkoffToken tinkoffToken, AccountId accountId, CancellationToken token)
    {
        var result = new List<Operation>();

        var response = new GetTinkoffOperationsResponse();
        do
        {
            response = await GetOperationsAsync(tinkoffToken, accountId, response.NextCursor, token);

            result.AddRange(response.Operations.Adapt<IEnumerable<Operation>>());
        }
        while (response.HasNext);

        return result;
    }

    private async Task<GetTinkoffOperationsResponse> GetOperationsAsync(TinkoffToken tinkoffToken, AccountId accountId, string? nextCursor, CancellationToken token)
    {
        var request = JsonSerializer.Serialize(new
        {
            account_id = accountId.ToString(),
            limit = 1_000,
            from = DateTime.Now.AddYears(-1).Date,
            to = DateTime.Now.Date,
            cursor = nextCursor
        });

        var content = new HttpRequestMessage
        {
            Content = new StringContent(request, Encoding.UTF8, "application/json"),
            RequestUri = new Uri(_tinkoffOperationsServiceUrl + "/GetOperationsByCursor"),
            Method = HttpMethod.Post
        };

        content.Headers.Add("Authorization", $"Bearer {tinkoffToken}");

        var response = await _client.SendAsync(content, token);

        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            throw new InvalidTokenException();
        }

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<GetTinkoffOperationsResponse>(cancellationToken: token);
    }

    private static bool AccountIsValid(TinkoffAccount account)
    {
        return account.Status == TinkoffAccountStatus.Open &&
              (account.Type == TinkoffAccountType.IIS || account.Type == TinkoffAccountType.Ordinary);
    }

    private async Task<Portfolio> GetPortfolioAsync(TinkoffAccount account, TinkoffToken tinkoffToken, CancellationToken token)
    {
        var content = new HttpRequestMessage
        {
            Content = new StringContent(JsonSerializer.Serialize(new { account_id = account.Id, currency = "RUB" }), Encoding.UTF8, "application/json"),
            RequestUri = new Uri(_tinkoffOperationsServiceUrl + "/GetPortfolio"),
            Method = HttpMethod.Post
        };

        content.Headers.Add("Authorization", $"Bearer {tinkoffToken}");

        var response = await _client.SendAsync(content, token);

        response.EnsureSuccessStatusCode();

        var serializedResponse = await response.Content.ReadFromJsonAsync<GetTinkoffPortfolioResponse>(cancellationToken: token);
        serializedResponse.Positions = serializedResponse.Positions.Where(x => x.Type == "bond");

        return (serializedResponse, account).Adapt<Portfolio>();
    }
}